using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Interfaces.AttractionInterface
{
    public interface IAttractionService
    {
        bool IsVisitorQueued(Attraction attraction, Guid visitorId);
        bool TryEnqueueVisitor(Attraction attraction, Visitor visitor);
        bool SetAttractionStatus(Attraction attraction, AttractionStatus newStatus);
        void ClearVisitorsFromAttraction(Attraction attraction);
    }
}
