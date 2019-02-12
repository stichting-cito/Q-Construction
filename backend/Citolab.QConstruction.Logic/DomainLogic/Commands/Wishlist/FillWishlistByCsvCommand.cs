using System;
using System.IO;
using Citolab.QConstruction.Logic.HelperClasses;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.Wishlist
{
   
    /// <summary>
    ///     Fill wishlist by csv
    /// </summary>
    public class FillWishlistByCsvCommand : IRequest<Model.Wishlist>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="csvStream"></param>
        /// <param name="title"></param>
        public FillWishlistByCsvCommand(Stream csvStream, string title)
        {
            CsvData = csvStream;
            Title = title;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="title"></param>
        public FillWishlistByCsvCommand(string resourceName, string title)
        {
            CsvData = GeneralHelper.LoadCsvWithData(resourceName);
            Title = title;
        }


        /// <summary>
        ///     adds
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="id"></param>
        /// <param name="daysToDeadline"></param>
        public FillWishlistByCsvCommand(string resourceName, Guid id, int? daysToDeadline)
        {
            DaysToDeadline = daysToDeadline;
            CsvData = GeneralHelper.LoadCsvWithData(resourceName);
            Id = id;
        }

        public FillWishlistByCsvCommand(Stream csvData, Guid id)
        {
            CsvData = csvData;
            Id = id;
        }

        /// <summary>
        ///     CsvData
        /// </summary>
        public Stream CsvData { get; }

        /// <summary>
        ///     Title
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// </summary>
        public Guid? Id { get; }
        /// <summary>
        /// Days to deadline, default deadline in csv
        /// </summary>
        public int? DaysToDeadline { get; set; }
    }
}