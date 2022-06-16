using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Image;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Publishes
{
  public class Publish : IPublish
  {
    private readonly IBus _bus;

    public Publish(
      IBus bus)
    {
      _bus = bus;
    }

    public async Task RemoveImagesAsync(List<Guid> imagesIds)
    {
      await _bus.Publish<IRemoveImagesPublish>(IRemoveImagesPublish.CreateObj(
        imagesIds: imagesIds,
        imageSource: ImageSource.User));
    }
  }
}
