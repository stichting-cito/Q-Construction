using System;
using System.IO;
using Citolab.QConstruction.Logic.HelperClasses;
using Citolab.QConstruction.Model;
using MediatR;

namespace Citolab.QConstruction.Logic.DomainLogic.Commands.ScreeningsList
{
    /// <summary>
    ///     Add screeningList by csv
    /// </summary>
    public class FillScreeningByCsvCommand : IRequest<ScreeningList>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="title"></param>
        public FillScreeningByCsvCommand(string resourceName, string title)
        {
            CsvData = GeneralHelper.LoadCsvWithData(resourceName);
            Title = title;
        }

        /// <summary>
        /// </summary>
        /// <param name="csvData"></param>
        /// <param name="id"></param>
        public FillScreeningByCsvCommand(Stream csvData, Guid id)
        {
            CsvData = csvData;
            Id = id;
        }

        /// <summary>
        ///     CsvData
        /// </summary>
        public Stream CsvData { get; }

        /// <summary>
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// </summary>
        public Guid? Id { get; }
    }
}