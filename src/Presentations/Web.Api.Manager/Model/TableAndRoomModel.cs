namespace Web.Api.Manager.Model
{
    public class TableAndRoomModel
    {
       
            public int Id { get; set; }
            public string Name { get; set; }
            public string AreaName { get; set; }
        
    }
    public class AreasModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberTable { get; set; }
        public List<TableAndRoomModel> TableAndRooms { get; set; }

    }
}
