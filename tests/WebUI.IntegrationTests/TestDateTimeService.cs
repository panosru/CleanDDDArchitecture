﻿using CleanArchitecture.Domain.Interfaces;
using System;

namespace CleanArchitecture.WebUI.IntegrationTests
{
    public class TestDateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
