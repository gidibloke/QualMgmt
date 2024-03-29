﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attributes
{
    public class NoFutureDatesAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            DateTime date = Convert.ToDateTime(value);
            return date <= DateTime.Now;
        }
    }
}
