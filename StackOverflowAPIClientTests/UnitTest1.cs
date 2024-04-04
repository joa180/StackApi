using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System;

namespace StackApi.Services.Tests
{
    public class StackOverflowAPIClientTests
    {
        [Fact]
        public async Task GetTags_SuccessfulResponse_ReturnsTags()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClient>();
            var httpClient = httpClientMock.Object;
            httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>()))
                          .ReturnsAsync(new HttpResponseMessage
                          {
                              StatusCode = System.Net.HttpStatusCode.OK,
                              Content = new StringContent("{\"items\": [{\"name\": \"tag1\", \"count\": 10}, {\"name\": \"tag2\", \"count\": 20}]}")
                          });

            var apiClient = new StackOverflowAPIClient(httpClientMock.Object, null);

            // Act
            var tags = await apiClient.GetTags();

            // Assert
            Assert.NotNull(tags);
            Assert.Contains("tag1", tags);
            Assert.Contains("tag2", tags);
        }

        [Fact]
        public async Task RefreshTags_SuccessfulResponse_ReturnsTags()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClient>();
            httpClientMock.Setup(client => client.GetAsync(It.IsAny<string>()))
                          .ReturnsAsync(new HttpResponseMessage
                          {
                              StatusCode = System.Net.HttpStatusCode.OK,
                              Content = new StringContent("{\"items\": [{\"name\": \"tag1\", \"count\": 10}, {\"name\": \"tag2\", \"count\": 20}]}")
                          });

            var apiClient = new StackOverflowAPIClient(httpClientMock.Object, null);

            // Act
            var tags = await apiClient.RefreshTags();

            // Assert
            Assert.NotNull(tags);
            Assert.Contains("tag1", tags);
            Assert.Contains("tag2", tags);
        }
    }
}
