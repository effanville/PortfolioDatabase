﻿using System;
using System.Xml.Serialization;

namespace FinancialStructures.StockStructures.Implementation
{
    /// <summary>
    /// Class containing all data pertaining to a stock.
    /// </summary>
    [XmlType(TypeName = "StockDay")]
    public class StockDay : IComparable<StockDay>
    {
        /// <summary>
        /// The start time of the interval this data is about.
        /// </summary>
        [XmlAttribute(AttributeName = "T")]
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// The length of time of the interval this data is about.
        /// </summary>
        [XmlAttribute(AttributeName = "D")]
        public TimeSpan Duration
        {
            get;
            set;
        } = TimeSpan.FromDays(1);

        /// <summary>
        /// The opening price in the interval.
        /// </summary>
        [XmlAttribute(AttributeName = "O")]
        public decimal Open
        {
            get;
            set;
        }

        /// <summary>
        /// The high value in this interval
        /// </summary>
        [XmlAttribute(AttributeName = "H")]
        public decimal High
        {
            get;
            set;
        }

        /// <summary>
        /// The low value in this interval.
        /// </summary>
        [XmlAttribute(AttributeName = "L")]
        public decimal Low
        {
            get;
            set;
        }

        /// <summary>
        /// The closing value in this interval.
        /// </summary>
        [XmlAttribute(AttributeName = "C")]
        public decimal Close
        {
            get;
            set;
        }

        /// <summary>
        /// The trading volume experienced in the interval.
        /// </summary>
        [XmlAttribute(AttributeName = "V")]
        public decimal Volume
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor setting nothing.
        /// </summary>
        public StockDay()
        {
        }

        /// <summary>
        /// Constructor setting all values.
        /// </summary>
        public StockDay(DateTime time, decimal open, decimal high, decimal low, decimal close, decimal volume)
        {
            Time = time;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        /// <summary>
        /// Get the relevant value based on the datastream.
        /// </summary>
        public decimal Value(StockDataStream data)
        {
            switch (data)
            {
                case StockDataStream.Open:
                    return Open;
                case StockDataStream.High:
                    return High;
                case StockDataStream.Low:
                    return Low;
                case StockDataStream.CloseOpen:
                    return Close / Open;
                case StockDataStream.HighOpen:
                    return High / Open;
                case StockDataStream.LowOpen:
                    return Low / Open;
                case StockDataStream.Volume:
                    return Volume;
                case StockDataStream.Close:
                default:
                    return Close;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Time}-O{Open}-H{High}-L{Low}-C{Close}-V{Volume}";
        }

        /// <inheritdoc/>
        public int CompareTo(StockDay obj)
        {
            if (obj is StockDay otherPrice)
            {
                return Time.CompareTo(otherPrice.Time);
            }

            return 0;
        }
    }
}
