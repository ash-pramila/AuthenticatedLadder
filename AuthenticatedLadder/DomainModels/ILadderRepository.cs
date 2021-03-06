﻿using System.Collections.Generic;

namespace AuthenticatedLadder.DomainModels
{
    public interface ILadderRepository
    {
        List<LadderEntry> GetTopEntries(string ladderId);
        LadderEntry Upsert(LadderEntry entry);
        LadderEntry GetEntryForUser(string ladderId, string platform, string username);
        List<LadderEntry> GetAllEntriesForLadder(string ladderId);
        bool DeleteEntry(string ladderId, string platform, string username);
    }
}