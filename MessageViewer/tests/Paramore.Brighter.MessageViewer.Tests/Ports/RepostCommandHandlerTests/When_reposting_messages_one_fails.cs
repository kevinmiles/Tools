﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using paramore.brighter.commandprocessor;
using Paramore.Brighter.MessageViewer.Ports.Handlers;
using Paramore.Brighter.MessageViewer.Tests.TestDoubles;
using Xunit;

namespace Paramore.Brighter.MessageViewer.Tests.Ports.RepostCommandHandlerTests
{
    public class RepostCommandHandlerMultipleMessagesOneFails
    {
        private string _storeName = "storeItemtestStoreName";
        private RepostCommandHandler _repostHandler;
        private RepostCommand _command;
        private Message _messageToRepost;
        private FakeMessageProducer _fakeMessageProducer;
        private Exception _ex;
        private Message _messageToRepostMissing;

        public RepostCommandHandlerMultipleMessagesOneFails()
        {
            var fakeStore = new FakeMessageStoreWithViewer();
            _messageToRepost = new Message(new MessageHeader(Guid.NewGuid(), "a topic", MessageType.MT_COMMAND, DateTime.UtcNow), new MessageBody("body"));
            fakeStore.Add(_messageToRepost);
            _messageToRepostMissing = new Message(new MessageHeader(Guid.NewGuid(), "a topic", MessageType.MT_COMMAND, DateTime.UtcNow), new MessageBody("body"));
            var fakeMessageStoreFactory = new FakeMessageStoreViewerFactory(fakeStore, _storeName);

            _command = new RepostCommand { MessageIds = new List<string> { _messageToRepost.Id.ToString(), _messageToRepostMissing.Id.ToString() }, StoreName = _storeName };
            _fakeMessageProducer = new FakeMessageProducer();
            _repostHandler = new RepostCommandHandler(fakeMessageStoreFactory, new FakeMessageProducerFactoryProvider(new FakeMessageProducerFactory(_fakeMessageProducer)), new MessageRecoverer());
        }

        [Fact]
        public void When_reposting_messages_one_fails()
        {
            _ex = Catch.Exception(() => _repostHandler.Handle(_command));

            //should_throw_expected_exception
            _ex.Should().BeOfType<Exception>();
            _ex.Message.Should().Contain("messages");
            _ex.Message.Should().Contain(_messageToRepostMissing.Id.ToString());
        }
   }
}