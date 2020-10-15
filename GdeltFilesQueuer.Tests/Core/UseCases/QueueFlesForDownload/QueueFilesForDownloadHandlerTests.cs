using GdeltFilesQueuer.Core.Services.QueueService;
using GdeltFilesQueuer.Core.UseCases.QueueFilesForDownload;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace GdeltFilesQueuer.Tests.Core.UseCases.QueueFlesForDownload
{
    public class QueueFilesForDownloadHandlerTests
    {
        private Mock<ILogger<QueueFilesForDownloadHandler>> logger;
        private Mock<IQueueService> queueService;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger<QueueFilesForDownloadHandler>>();
            queueService = new Mock<IQueueService>();
        }

        [Test]
        public void ArgumentNullException_for_null_request()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Test]
        public void ArgumentNullException_for_null_startDatet()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                EndDate = new DateTime(2020, 01, 10)
            };

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request));
        }

        [Test]
        public void ArgumentNullException_for_null_endDatet()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2020, 01, 10)
            };

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(request));
        }

        [Test]
        public void ArgumentException_when_start_date_is_greater_than_end_date()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2020, 02, 10),
                EndDate = new DateTime(2020, 01, 10)
            };

            Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request));
        }

        [Test]
        public void ArgumentException_when_start_date_is_less_than_1979()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(1978, 12, 31),
                EndDate = new DateTime(2020, 01, 10)
            };

            Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request));
        }

        [Test]
        public void ArgumentException_when_end_date_is_today()
        {
            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2020, 02, 10),
                EndDate = DateTime.Now
            };

            Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(request));
        }

        [Test]
        public async Task Handler_should_queue_correct_number_of_messages()
        {
            queueService.Setup(x => x.Queue(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 10)
            };

            await sut.Handle(request);

            queueService.Verify(x => x.Queue(It.IsAny<Message>()), Times.Exactly(10));
        }

        [Test]
        public async Task Handler_should_queue_message_with_yyyyMMdd_format()
        {
            queueService.Setup(x => x.Queue(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2013, 04, 01),
                EndDate = new DateTime(2013, 04, 03)
            };

            await sut.Handle(request);

            queueService.Verify(x => x.Queue(It.Is<Message>(y => string.Equals(y.DownloadUrl, "http://data.gdeltproject.org/events/20130401.export.CSV.zip"))), Times.Exactly(1));
            queueService.Verify(x => x.Queue(It.Is<Message>(y => string.Equals(y.DownloadUrl, "http://data.gdeltproject.org/events/20130402.export.CSV.zip"))), Times.Exactly(1));
            queueService.Verify(x => x.Queue(It.Is<Message>(y => string.Equals(y.DownloadUrl, "http://data.gdeltproject.org/events/20130403.export.CSV.zip"))), Times.Exactly(1));
        }

        [Test]
        public async Task Handler_should_queue_message_with_yyyyMM_format()
        {
            queueService.Setup(x => x.Queue(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2006, 01, 01),
                EndDate = new DateTime(2006, 01, 03)
            };

            await sut.Handle(request);

            queueService.Verify(x => x.Queue(It.Is<Message>(y => string.Equals(y.DownloadUrl, "http://data.gdeltproject.org/events/200601.export.CSV.zip"))), Times.Exactly(3));
        }

        [Test]
        public async Task Handler_should_queue_message_with_yyyy_format()
        {
            queueService.Setup(x => x.Queue(It.IsAny<Message>())).Returns(Task.CompletedTask);

            var sut = new QueueFilesForDownloadHandler(logger.Object, queueService.Object);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = new DateTime(2005, 12, 29),
                EndDate = new DateTime(2005, 12, 31)
            };

            await sut.Handle(request);

            queueService.Verify(x => x.Queue(It.Is<Message>(y => string.Equals(y.DownloadUrl, "http://data.gdeltproject.org/events/2005.export.CSV.zip"))), Times.Exactly(3));
        }
    }
}
