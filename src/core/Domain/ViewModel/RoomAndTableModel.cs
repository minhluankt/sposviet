using System;

namespace Domain.ViewModel
{
    public class RoomAndTableModel
    {
        public Guid? Idtable { get; set; }
        public bool IsOrder { get; set; }// bàn dg đặt
        public bool IsBringBack { get; set; }// bàn  mnag về
        public DateTime CreateDate { get; set; }// thời gian bắt đ
        public int TimeNumber { get; set; }// thời gian giây
        public string FullAmount { get; set; }// Tổng tiền
        public string StaffName { get; set; }// nhân viên phục vụ
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
