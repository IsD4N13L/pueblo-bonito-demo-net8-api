﻿namespace PuebloBonitoApi.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException() : base()
        {
        }
        public InternalServerErrorException(string message) : base(message)
        {
        }
    }
}
