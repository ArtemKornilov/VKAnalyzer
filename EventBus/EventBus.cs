using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
	public class EventBus : IEventBus
	{
		private readonly IConnection _connection;
		private IModel _channel;

		private readonly IServiceProvider _provider;

		public IModel Channel
		{
			get
			{
				if (_channel == null)
				{
					_channel = _connection.CreateModel();
				}

				return _channel;
			}
		}

		public EventBus(IConfiguration config, IServiceProvider provider)
		{
			var connectionFactory = new ConnectionFactory()
			{
				HostName = config["EventBus:HostName"],
				UserName = config["EventBus:UserName"],
				Password = config["EventBus:Password"],
				DispatchConsumersAsync = true
			};

			_connection = connectionFactory.CreateConnection();
			_provider = provider;
		}

		public void Publish(IIntegrationEvent @event, string exchangeName)
		{
			CreateExchange(exchangeName);

			var jsonEvent = JsonConvert.SerializeObject(@event);
			var encodedEvent = Encoding.UTF8.GetBytes(jsonEvent);

			Channel.BasicPublish(exchangeName, string.Empty, body: encodedEvent);
		}

		public void Subscribe<TH, TE>(string exchangeName, string subscriberName) where TH : IIntegrationEventHandler<TE>
																				  where TE : IIntegrationEvent
		{
			BindQueue(exchangeName, subscriberName);

			var consumer = new AsyncEventingBasicConsumer(Channel);

			consumer.Received += async (obj, args) =>
			{
				using (var scope = _provider.CreateScope())
				{
					var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TE>>();

					var jsonMessage = Encoding.UTF8.GetString(args.Body.ToArray());
					var message = JsonConvert.DeserializeObject<TE>(jsonMessage);

					await handler.HandleAsync(message);

					Channel.BasicAck(args.DeliveryTag, multiple: false);
				}
			};

			Channel.BasicConsume(subscriberName, false, consumer);
		}

		private void CreateExchange(string exchangeName)
		{
			Channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true);
		}

		private void BindQueue(string exchangeName, string subscriberName) 
		{
			CreateExchange(exchangeName);

			Channel.QueueDeclare(subscriberName, durable: false, exclusive: false, autoDelete: false);
			Channel.QueueBind(subscriberName, exchangeName, string.Empty);
		}
	}
}
