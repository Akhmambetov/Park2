using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Interfaces.AttractionInterface
{
    public interface IRandomAttractionService
    {
        Attraction? SelectAttraction(Visitor visitor, IEnumerable<Attraction> attractions);
    }
}
