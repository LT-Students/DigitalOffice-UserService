﻿using System;

namespace LT.DigitalOffice.UserService.Models.Dto
{
    public class Achievement
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public Guid PictureFileId { get; set; }
    }
}