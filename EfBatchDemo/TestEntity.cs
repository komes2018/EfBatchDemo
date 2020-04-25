namespace EfBatchDemo
{
    using System.ComponentModel.DataAnnotations;

    public class TestEntity
    {
        [Key]
        public int Id { get; set; }

        public int RandomVlue { get; set; }
    }
}