using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Helpers;

internal record TestContainer(TestcontainersContainer Docker, int ConnectionPort);