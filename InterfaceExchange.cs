using Burcat.API.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burcat.API
{
    public static class InterfaceExchange
    {
        public static Func<IQueryable<IPost>, string?, IQueryable<IPost>> DoPostSearch { get; set; } = (a, b) => a;
        public static Func<IQueryable<Faction>, string?, IQueryable<Faction>> DoFactionSearch { get; set; } = (a, b) => a;
    }
}
