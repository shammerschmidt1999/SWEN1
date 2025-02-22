﻿using SWEN1_MCTG.Classes;

namespace SWEN1_MCTG.Data.Repositories.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Task<List<Card>> GetRandomCardsAsync(int count);
}