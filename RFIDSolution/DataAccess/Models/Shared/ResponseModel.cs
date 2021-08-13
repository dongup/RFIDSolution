﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Models
{
    public class ResponseModel<T>
    {
        public bool IsSucceed { get; set; }
        public T Result { get; set; } 
        public string Message { get; set; }
        public int StatusCode { get; set; } = 200;
        public MyException Exception { get; set; } = new MyException();

        public ResponseModel<T> Succeed(T Result)
        {
            IsSucceed = true;
            this.Result = Result;
            return this;
        }

        public ResponseModel<T> Succeed()
        {
            IsSucceed = true;
            return this;
        }

        public ResponseModel<T> Failed(string msg = "")
        {
            IsSucceed = false;
            Message = msg;
            return this;
        }

        public ResponseModel<T> NotFound()
        {
            Message = "This item does not existed!";
            IsSucceed = false;
            return this;
        }

        public ResponseModel<T> NotFound(T result)
        {
            Message = "This item does not existed!";
            IsSucceed = false;
            Result = result;
            return this;
        }

        public ResponseModel<T> NotFound(string msg)
        {
            Message = msg;
            IsSucceed = false;
            return this;
        }

        public ResponseModel<T> Forbiden()
        {
            Message = "Access denied! You do not have permission to access.";
            IsSucceed = false;
            return this;
        }

        public ResponseModel<T> UnAuthorize()
        {
            Message = "Please login first!";
            IsSucceed = false;
            return this;
        }
    }

    public class MyException
    {
        public string Message { get; set; }

        public string InnerMessage { get; set; }

        public string StackTrace { get; set; }
    }
}
