using System;

namespace Domain.ViewModel
{
    public class RoomAndTableModel
    {
        public Guid? Idtable { get; set; }
        public bool IsOrder { get; set; }// bàn dg đặt
        public bool IsBringBack { get; set; }// bàn  mnag về
    }
    public class RoomAndTableViewModel
    {
        public Guid idtable { get; set; }
        public string tableName { get; set; }
        public int idArea { get; set; }
        public int numberProduct { get; set; }
        public string nameArea { get; set; }
       
    }
}
