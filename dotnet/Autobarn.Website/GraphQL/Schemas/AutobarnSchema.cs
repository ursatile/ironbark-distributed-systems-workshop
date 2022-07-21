using AutoMate.Data;
using Autobarn.Website.GraphQL.Queries;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.Schemas {
    public class AutobarnSchema : Schema {
        public AutobarnSchema(IAutoMateDatabase db) => Query = new VehicleQuery(db);
    }
}
