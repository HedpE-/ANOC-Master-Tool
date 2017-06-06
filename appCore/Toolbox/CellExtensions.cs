using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using appCore.SiteFinder;

namespace appCore
{
    public static class CellExtension
    {
        public static List<Cell> Filter(this List<Cell> toFilter, Cell.Filters filter)
        {
            List<Cell> list = new List<Cell>();
            if (toFilter != null)
            {
                switch (filter)
                {
                    case Cell.Filters.All_2G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.GSM && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.VF_2G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.GSM && cell.Operator == Operators.Vodafone && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.TF_2G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.GSM && cell.Operator == Operators.Telefonica && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.All_3G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.UMTS && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.VF_3G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.UMTS && cell.Operator == Operators.Vodafone && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.TF_3G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.UMTS && cell.Operator == Operators.Telefonica && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.All_4G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.LTE && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.VF_4G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.LTE && cell.Operator == Operators.Vodafone && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.TF_4G:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Bearer == Bearers.LTE && cell.Operator == Operators.Telefonica && cell.Noc.Contains("ANOC"))
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.Locked:
                        foreach (Cell cell in toFilter)
                        {
                            if (cell.Locked)
                                list.Add(cell);
                        }
                        return list;
                    case Cell.Filters.Unlocked:
                        foreach (Cell cell in toFilter)
                        {
                            if (!cell.Locked)
                                list.Add(cell);
                        }
                        return list;
                }
            }
            return list;
        }
    }
}
