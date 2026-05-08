using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using ProviderDefinition = CluedIn.Core.Data.Relational.ProviderDefinition;
using CluedIn.Core.DataStore;
using CluedIn.Core.ExternalSearch;
using CluedIn.Core.Messages.Processing;
using CluedIn.Core.Processing;
using CluedIn.Core.Providers;
using CluedIn.Core.Serialization;
using CluedIn.Core.Workflows;
using CluedIn.ExternalSearch;
using CluedIn.ExternalSearch.Providers.CompanyHouse;
using CluedIn.ExternalSearch.Providers.CompanyHouse.Model;
using CluedIn.Testing.Base.ExternalSearch;
using CluedIn.Testing.Base.Processing.Actors;
using Moq;
using Xunit;
using TestContext = CluedIn.Testing.Base.Context.TestContext;

namespace ExternalSearch.CompanyHouse.Integration.Tests
{
    public class CompanyHouseGraphTests : BaseExternalSearchTest<CompanyHouseExternalSearchProvider>
    {
        [Fact]
        public void Test()
        {
            // Arrange
            testContext = new TestContext();

            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, "http://sitecore.net");
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.AddressCountryCode, "uk");
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, "Sitecore");
            properties.Properties.Add("Website", "http://sitecore.net.com");

            IEntityMetadata entityMetadata = new EntityMetadataPart()
            {
                Name = "Sitecore",
                EntityType = EntityType.Organization,
                OriginEntityCode = new EntityCode(EntityType.Organization, CodeOrigin.CluedIn, "Sitecore"),
                Properties = properties.Properties
            };

            var externalSearchProvider = new Mock<CompanyHouseExternalSearchProvider>(MockBehavior.Loose);
            var clues = new List<CompressedClue>();

            externalSearchProvider.CallBase = true;
            externalSearchProvider.As<IConfigurableExternalSearchProvider>()
                .Setup(p => p.ExecuteSearch(It.IsAny<ExecutionContext>(), It.IsAny<IExternalSearchQuery>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<IProvider>()))
                .Returns((ExecutionContext ctx, IExternalSearchQuery q, IDictionary<string, object> c, IProvider p) =>
                    new IExternalSearchQueryResult[] { new ExternalSearchQueryResult<CompanyNew>(q, new CompanyNew { company_name = "Sitecore", company_number = "12345678" }) });

            testContext.ProcessingHub
                .Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>()))
                .Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));
            testContext.ProcessingHub
                .Setup(h => h.SendCommandAsync(It.IsAny<ProcessClueCommand>()))
                .Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue))
                .Returns(Task.CompletedTask);

            testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => externalSearchProvider.Object));

            var providerDefinition = new ProviderDefinition { Id = Guid.NewGuid(), IsEnabled = true };
            var mockProviderProvider = new Mock<IExternalSearchProviderProvider>();
            mockProviderProvider.SetupGet(p => p.ExternalSearchProvider).Returns(externalSearchProvider.Object);
            var mockProviderConfig = new Mock<IExternalSearchProviderConfiguration>();
            mockProviderConfig.SetupGet(c => c.ExternalSearchProvider).Returns(externalSearchProvider.Object);
            mockProviderConfig.SetupGet(c => c.Provider).Returns(mockProviderProvider.As<IProvider>().Object);
            mockProviderConfig.SetupGet(c => c.ProviderDefinition).Returns(providerDefinition);
            testContext.ExternalSearchProvidersRepository
                .Setup(r => r.GetProviderConfigurations(It.IsAny<ExecutionContext>(), It.IsAny<IExternalSearchRequest>()))
                .Returns(new[] { mockProviderConfig.Object });
            testContext.ExternalSearchProvidersRepository
                .Setup(r => r.GetProviderConfigurations(It.IsAny<ExecutionContext>()))
                .Returns(new[] { mockProviderConfig.Object });
            testContext.ExternalSearchProvidersRepository
                .Setup(r => r.GetProviderConfiguration(It.IsAny<ExecutionContext>(), It.IsAny<Guid>()))
                .Returns(mockProviderConfig.Object);

            var mockConfigRepo = new Mock<IConfigurationRepository>();
            mockConfigRepo
                .Setup(r => r.GetConfigurationById(It.IsAny<ExecutionContext>(), It.IsAny<Guid>()))
                .Returns(new Dictionary<string, object>
                {
                    { CluedIn.ExternalSearch.Providers.CompanyHouse.Constants.KeyName.AcceptedEntityType, EntityType.Organization.ToString() }
                });
            testContext.Container.Register(Component.For<IConfigurationRepository>().Instance(mockConfigRepo.Object));

            var context = testContext.Context.ToProcessingContext();
            var command = new ExternalSearchCommand();
            var actor = new ExternalSearchProcessingAccessor(context.ApplicationContext);
            var workflow = new Mock<Workflow>(MockBehavior.Loose, context,
                new EmptyWorkflowTemplate<ExternalSearchCommand>())
            {
                CallBase = true
            };

            command.With(context);
            command.OrganizationId = context.Organization.Id;
            command.EntityMetaData = entityMetadata;
            command.Workflow = workflow.Object;
            context.Workflow = command.Workflow;

            // Act
            var result = actor.ProcessWorkflowStepAsync(context, command).GetAwaiter().GetResult();
            Assert.Equal(WorkflowStepResult.Repeat.SaveResult, result.SaveResult);

            result = actor.ProcessWorkflowStepAsync(context, command).GetAwaiter().GetResult();
            Assert.Equal(WorkflowStepResult.Success.SaveResult, result.SaveResult);
            context.Workflow.AddStepResult(result);

            context.Workflow.ProcessStepResult(context, command);

            // Assert
            testContext.ProcessingHub.Verify(h => h.SendCommandAsync(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);

            Assert.True(clues.Count > 0);
        }
    }
}
