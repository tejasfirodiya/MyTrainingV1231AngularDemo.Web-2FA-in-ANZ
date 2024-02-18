using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniExcelLibs;

namespace MyTrainingV1231AngularDemo.DataExporting.Excel.MiniExcel
{
    public abstract class MiniExcelExcelImporterBase<TEntity>
    {
        protected List<TEntity> ProcessExcelFile(byte[] fileBytes, Func<dynamic, TEntity> processExcelRow,
            bool useOldExcelFormat = false)
        {
            var entities = new List<TEntity>();

            using (var stream = new MemoryStream(fileBytes))
            {
                var rows = stream.Query(useHeaderRow:true).ToList();
                foreach (var row in rows)
                {
                    var entitiesInWorksheet = ProcessWorksheet(row, processExcelRow);
                    entities.AddRange(entitiesInWorksheet);
                }
            }

            return entities;
        }

        private List<TEntity> ProcessWorksheet(dynamic row, Func<dynamic, TEntity> processExcelRow)
        {
            var entities = new List<TEntity>();

            try
            {
                var entity = processExcelRow(row);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return entities;
        }
    }
}