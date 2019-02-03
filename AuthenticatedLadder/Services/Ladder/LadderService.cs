﻿using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AuthenticatedLadder.Services.Ladder
{
    public class LadderService : ILadderService
    {
        private ILadderRepository _repository;
        private ILoggerAdapter<LadderService> _logger;

        public LadderService(ILadderRepository repository, ILoggerAdapter<LadderService> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        private bool isValidGetEntryForUserInput(string ladderId, string platform, string username)
        {
            return !string.IsNullOrEmpty(ladderId)
                && !string.IsNullOrEmpty(platform)
                && !string.IsNullOrEmpty(username);
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            LadderEntry result = null;
            if (isValidGetEntryForUserInput(ladderId, platform, username))
            {
                try
                {
                    result = _repository.GetEntryForUser(ladderId, platform, username);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "GetEntryForUser failed to read from repository");
                    throw;
                }
            }
            return result;
        }

        public List<LadderEntry> GetTopEntries(string ladderId)
        {
            var result = new List<LadderEntry>();

            if (!string.IsNullOrEmpty(ladderId))
            {
                try
                {
                    result = _repository.GetTopEntries(ladderId);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "GetTopEntries failed to read from repository");
                    throw;
                }
            }
            return result;
        }

        public LadderEntry Upsert(LadderEntry entry)
        {
            LadderEntry result = null;
            if (entry != null)
            {
                try
                {
                    result = _repository.Upsert(entry);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Upsert failed to read from repository");
                    throw;
                }
            }
            return result;
        }
    }
}
