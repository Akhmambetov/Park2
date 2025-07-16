using Park2.Domain.Enums;
using Park2.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Park2.Application.Interfaces.VisitorInterface
{
    public interface IVisitorService
    {
        bool ShouldLeavePark(Visitor visitor, DateTime currentTime);
        VisitorRoutingResult RouteVisitor(Visitor visitor, IEnumerable<Attraction> attractions);
        Visitor CreateRandomVisitor(DateTime currentTime, IEnumerable<Attraction> attractions);
        public Visitor CreateVisitorFromConsole(
                DateTime currentTime,
                string name,
                int age,
                bool isVIP,
                Attraction chosenAttraction);
        void UpdatePreferredAttraction(Visitor visitor, IEnumerable<Attraction> attractions);
        void RouteActiveVisitorsFromAttraction(List<Attraction> attractions, List<Visitor> visitors);
    }
}
