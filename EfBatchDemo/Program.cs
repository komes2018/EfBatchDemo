using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfBatchDemo
{
    using System.Collections.Concurrent;
    using System.Diagnostics;

    using EntityFramework.Utilities;

    class Program
    {
        static void Main(string[] args)
        {
            BatchInster();
            //BatchUpdate();
            //BatchUpdateQuery();
            //BatchDelete();
            //Test111
            Console.ReadKey();
        }

        /// <summary>
        ///     批量插入
        /// </summary>
        private static void BatchInster()
        {
            var datas = GetInsertDatas();
            var testEntities = datas as IList<TestEntity> ?? datas.ToList();

            Stopwatch watch =new Stopwatch();
            
            Console.WriteLine("开始插入计时,总共数据:{0}条",testEntities.Count());
            watch.Start();

            using (var context=new BatchDemoContext())
            {
                EFBatchOperation.For(context,context.TestEntities)
                    .InsertAll(testEntities);
            }

            watch.Stop();
            Console.WriteLine("结束插入计时,工用时:{0}ms",watch.ElapsedMilliseconds);


            using (var context = new BatchDemoContext())
            {
                var count = context.TestEntities.Count();
                Console.WriteLine("数据库总共数据:{0}条",count);

                var minId = context.TestEntities.Min(c => c.Id);

                // 随机取十条数据进行验证

                for (int i = 1; i <= 10; i++)
                {
                    Random rand = new Random();
                    var id = rand.Next(minId, minId+ 100000);
                    
                    var testdata = context.TestEntities.FirstOrDefault(c => c.Id == id);
                    
                    Console.WriteLine("插入的数据 id:{0} randomvalue:{1}",testdata.Id,testdata.RandomVlue);

                }
                
            }


            Console.WriteLine("-----------------华丽的分割线   插入-------------------------");
        }

        /// <summary>
        ///     批量更新
        /// </summary>
        private static void BatchUpdate()
        {
            IEnumerable<TestEntity> toUpdates=new List<TestEntity>();

            // 获取所有数据
            using (var context = new BatchDemoContext())
            {
                toUpdates = context.TestEntities.ToList();
            }

            // 所有的值 都为 1000
            Parallel.ForEach(toUpdates,
                (entity, state) =>
                    { entity.RandomVlue = 1000; });


            Stopwatch watch = new Stopwatch();

            Console.WriteLine("开始更新计时,总共数据:{0}条", toUpdates.Count());
            watch.Start();

            using (var context = new BatchDemoContext())
            {
                EFBatchOperation.For(context, context.TestEntities).UpdateAll(toUpdates, x => x.ColumnsToUpdate(c => c.RandomVlue));
            }

            watch.Stop();
            Console.WriteLine("结束更新计时,工用时:{0}ms", watch.ElapsedMilliseconds);


            using (var context = new BatchDemoContext())
            {
                var count = context.TestEntities.Count();
                Console.WriteLine("数据库总共数据:{0}条", count);

                var minId = context.TestEntities.Min(c => c.Id);

                // 随机取十条数据进行验证

                for (int i = 1; i <= 10; i++)
                {
                    Random Rand = new Random();
                    var id = Rand.Next(minId, minId+100000);

                    var testdata = context.TestEntities.FirstOrDefault(c => c.Id == id);
                    Console.WriteLine("更新的数据 id:{0} randomvalue:{1}", testdata.Id, testdata.RandomVlue);

                }

            }


            Console.WriteLine("-----------------华丽的分割线   更新-------------------------");

        }

        /// <summary>
        ///     将id >= 1w  小于 5w 的随机值等于 500
        /// </summary>
        private static void BatchUpdateQuery()
        {

            Stopwatch watch = new Stopwatch();

            Console.WriteLine("开始查询更新计时");
            watch.Start();

            using (var context = new BatchDemoContext())
            {

                var minId = context.TestEntities.Min(c => c.Id);

                EFBatchOperation.For(context, context.TestEntities)
                    .Where(c=>c.Id>= minId+10000 && c.Id<= minId+50000)
                    .Update(c=>c.RandomVlue,rv=>500);
            }

            watch.Stop();
            Console.WriteLine("结束查询更新计时,工用时:{0}ms", watch.ElapsedMilliseconds);

            using (var context = new BatchDemoContext())
            {
                var count = context.TestEntities.Count();
                Console.WriteLine("数据库总共数据:{0}条", count);

                var minId = context.TestEntities.Min(c => c.Id);
                // 随机取十条数据进行验证
                for (int i = 1; i <= 10; i++)
                {
                    Random rand = new Random();
                    var id = rand.Next(minId+10000, minId+ 50000);

                    var testdata = context.TestEntities.FirstOrDefault(c => c.Id == id);
                    Console.WriteLine("查询更新的数据 id:{0} randomvalue:{1}", testdata.Id, testdata.RandomVlue);

                }

            }


            Console.WriteLine("-----------------华丽的分割线  查询更新-------------------------");


        }


        /// <summary>
        ///     删除所有数据
        /// </summary>
        private static void BatchDelete()
        {

            Stopwatch watch = new Stopwatch();

            Console.WriteLine("开始删除计时");
            watch.Start();

            using (var context = new BatchDemoContext())
            {
                EFBatchOperation.For(context, context.TestEntities)
                    .Where(c=>c.Id>=1).Delete();
            }

            watch.Stop();
            Console.WriteLine("结束删除计时,工用时:{0}ms", watch.ElapsedMilliseconds);

            using (var context = new BatchDemoContext())
            {
                var count = context.TestEntities.Count();
                Console.WriteLine("数据库总共数据:{0}条", count);
                
            }

            Console.WriteLine("-----------------华丽的分割线  删除-------------------------");

        }


        /// <summary>
        ///     产生需要生产的数据
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<TestEntity> GetInsertDatas()
        {
            // 线程安全的list
            ConcurrentBag<TestEntity> datas=new ConcurrentBag<TestEntity>();

            Parallel.For(0, 100000,
                (index, state) =>
                    {
                        Random rand = new Random();
                        var newData = new TestEntity { RandomVlue = rand.Next(1, 100) };
                        datas.Add(newData);
                    });

            return datas;
        }

        private static IEnumerable<TestEntity> GetInsertDatas1()
        {
            // 线程安全的list
            ConcurrentBag<TestEntity> datas = new ConcurrentBag<TestEntity>();

            Parallel.For(0, 100000,
                (index, state) =>
                {
                    Random rand = new Random();
                    var newData = new TestEntity { RandomVlue = rand.Next(1, 100) };
                    datas.Add(newData);
                });

            return datas;
        }

    }
}
