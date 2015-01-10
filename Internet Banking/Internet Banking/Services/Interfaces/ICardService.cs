using System;
using System.Collections.Generic;
using Internet_Banking.Models;

namespace Internet_Banking.Services.Interfaces
{
    interface ICardService
    {
        CardDetailModel CreateModel();
        void Save(CardDetailModel model);
        void FillLists(CardDetailModel model);
        List<CardDetailModel> GetCards(Guid userId);
        CardDetailModel GetCard(Guid id);
        CardDetailModel GetCard(string cardNumber);
        void ChangeNumber(CardDetailModel card);
    }
}