namespace EfBatchDemo
{
    using System.Data.Entity;

    public class BatchDemoContext
        : DbContext
    {
        public BatchDemoContext() : 
            base("Default")
        {
        }

        public IDbSet<TestEntity> TestEntities { get; set; }



    }
}