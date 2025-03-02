using System.Collections.Generic;

public static class Holidays
{
    public static readonly Dictionary<int, List<(int Month, int Day, string Date, string Name)>> HolidayDates =
        new Dictionary<int, List<(int, int, string, string)>>
        {
            {
                2065, new List<(int, int, string, string)>
                {
                    (1, 11, "2008-04-23", "Loktantra Diwas"), // Baisakh
                    (2, 7, "2008-05-20", "Buddha Jayanti"), // Jestha
                    (5, 7, "2008-08-23", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 8, "2008-08-24", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 17, "2008-09-02", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 19, "2008-09-04", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 22, "2008-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 14, "2008-09-30", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 20, "2008-10-06", "Phulpati"), // Ashwin
                    (6, 21, "2008-10-07", "Mahaastami Barta"), // Ashwin
                    (6, 22, "2008-10-08", "Mahanawami"), // Ashwin
                    (6, 23, "2008-10-09", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 12, "2008-10-28", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 14, "2008-10-30", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 19, "2008-11-04", "Chhath Parba"), // Kartik
                    (9, 10, "2008-12-25", "Christmas (Christian only)"), // Poush
                    (10, 1, "2009-01-14", "Maghe Sankranti "), // Magh
                    (10, 1, "2009-01-14", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 14, "2009-01-27", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2009-02-18", "National Democracy Day"), // Falgun
                    (11, 9, "2009-02-20", "Fagu Purnima"), // Falgun
                    (11, 27, "2009-03-10", "Fagu Purnima"), // Falgun
                    (12, 21, "2009-04-03", "Raam Nawami Brat (Raam Jayanti)") // Chaitra
                }
            },
            {
                2066, new List<(int, int, string, string)>
                {
                    (1, 11, "2009-04-24", "Loktantra Diwas"), // Baisakh
                    (1, 26, "2009-05-09", "Buddha Jayanti"), // Baisakh
                    (2, 15, "2009-05-29", "Republic Day"), // Jestha
                    (4, 29, "2009-08-13", "Shree Krishna Janmasthami Brata (Moharatri)"), // Shrawan
                    (5, 7, "2009-08-23", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 8, "2009-08-24", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 18, "2009-09-03",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (5, 22, "2009-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 3, "2009-09-19", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 9, "2009-09-25", "Phulpati"), // Ashwin
                    (6, 10, "2009-09-26", "Mahaastami Barta"), // Ashwin
                    (6, 11, "2009-09-27", "Mahanawami"), // Ashwin
                    (6, 12, "2009-09-28", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 17, "2009-10-03", "Kojagratbrat"), // Ashwin
                    (6, 18, "2009-10-04", "Kojagratbrat"), // Ashwin
                    (6, 31, "2009-10-17", "Dipawali (Laxmi Puja)"), // Ashwin
                    (7, 1, "2009-10-18", "Mha Pooja"), // Kartik
                    (7, 2, "2009-10-19", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 7, "2009-10-24", "Chhath Parba"), // Kartik
                    (7, 18, "2009-11-04", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (8, 17, "2009-12-02", "Udhauli Parwa"), // Mangsir
                    (9, 10, "2009-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2009-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (10, 1, "2010-01-15", "Maghe Sankranti "), // Magh
                    (10, 2, "2010-01-16", "Sonam (Tamang) Lhosar"), // Magh
                    (10, 16, "2010-01-30", "Martyrs' Day"), // Magh
                    (10, 29, "2010-02-12", "Maha Shivaratri Chaturdasi Brat"), // Magh
                    (11, 2, "2010-02-14", "Gyalpo Lhosar"), // Falgun
                    (11, 7, "2010-02-19", "National Democracy Day"), // Falgun
                    (11, 16, "2010-02-28", "Fagu Purnima"), // Falgun
                    (12, 11, "2010-03-24", "Raam Nawami Brat (Raam Jayanti)") // Chaitra
                }
            },
            {
                2067, new List<(int, int, string, string)>
                {
                    (1, 11, "2010-04-24", "Loktantra Diwas"), // Baisakh
                    (2, 13, "2010-05-27", "Buddha Jayanti"), // Jestha
                    (2, 15, "2010-05-29", "Republic Day"), // Jestha
                    (5, 8, "2010-08-24", "Rishitarpani (Janai Purnima)"), // Bhadra
                    (5, 16, "2010-09-01", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 22, "2010-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (5, 26, "2010-09-11", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 27, "2010-09-12", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (6, 22, "2010-10-08", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 28, "2010-10-14", "Phulpati"), // Ashwin
                    (6, 29, "2010-10-15", "Kaal Ratri"), // Ashwin
                    (6, 31, "2010-10-17", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 19, "2010-11-05", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 20, "2010-11-06", "Mha Pooja"), // Kartik
                    (7, 21, "2010-11-07", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 25, "2010-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 26, "2010-11-12", "Chhath Parba"), // Kartik
                    (8, 5, "2010-11-21", "Guru Nanak Jayanti (Sikh only)"), // Mangsir
                    (9, 10, "2010-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2010-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (10, 1, "2011-01-15", "Maghe Sankranti "), // Magh
                    (10, 1, "2011-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 16, "2011-01-30", "Martyrs' Day"), // Magh
                    (10, 21, "2011-02-04", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2011-02-19", "National Democracy Day"), // Falgun
                    (11, 18, "2011-03-02", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 21, "2011-03-05", "Gyalpo Lhosar"), // Falgun
                    (12, 5, "2011-03-19", "Fagu Purnima"), // Chaitra
                    (12, 29, "2011-04-12", "Raam Nawami Brat (Raam Jayanti)") // Chaitra
                }
            },
            {
                2068, new List<(int, int, string, string)>
                {
                    (1, 3, "2011-04-16", "Mahabir Jayanti (Jain only)"), // Baisakh
                    (1, 11, "2011-04-24", "Loktantra Diwas"), // Baisakh
                    (2, 3, "2011-05-17", "Buddha Jayanti"), // Jestha
                    (4, 28, "2011-08-13", "Rishitarpani (Janai Purnima)"), // Shrawan
                    (5, 4, "2011-08-21", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 14, "2011-08-31", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 16, "2011-09-02", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 22, "2011-09-08", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 11, "2011-09-28", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 16, "2011-10-03", "Phulpati"), // Ashwin
                    (6, 17, "2011-10-04", "Mahaastami Barta"), // Ashwin
                    (6, 17, "2011-10-04", "Kaal Ratri"), // Ashwin
                    (6, 18, "2011-10-05", "Mahanawami"), // Ashwin
                    (6, 19, "2011-10-06", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 24, "2011-10-11", "Kojagratbrat"), // Ashwin
                    (7, 9, "2011-10-26", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 10, "2011-10-27", "Mha Pooja"), // Kartik
                    (7, 11, "2011-10-28", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 15, "2011-11-01", "Chhath Parba"), // Kartik
                    (7, 24, "2011-11-10", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (9, 10, "2011-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2011-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2012-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (10, 1, "2012-01-15", "Maghe Sankranti "), // Magh
                    (10, 10, "2012-01-24", "Sonam (Tamang) Lhosar"), // Magh
                    (10, 16, "2012-01-30", "Martyrs' Day"), // Magh
                    (11, 7, "2012-02-19", "National Democracy Day"), // Falgun
                    (11, 8, "2012-02-20", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 10, "2012-02-22", "Gyalpo Lhosar"), // Falgun
                    (11, 24, "2012-03-07", "Fagu Purnima"), // Falgun
                    (12, 18, "2012-03-31", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 22, "2012-04-04", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2069, new List<(int, int, string, string)>
                {
                    (1, 3, "2011-04-16", "Mahabir Jayanti (Jain only)"), // Baisakh
                    (1, 11, "2011-04-24", "Loktantra Diwas"), // Baisakh
                    (2, 3, "2011-05-17", "Buddha Jayanti"), // Jestha
                    (4, 28, "2011-08-13", "Rishitarpani (Janai Purnima)"), // Shrawan
                    (5, 4, "2011-08-21", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 14, "2011-08-31", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 16, "2011-09-02", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 22, "2011-09-08", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 11, "2011-09-28", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 16, "2011-10-03", "Phulpati"), // Ashwin
                    (6, 17, "2011-10-04", "Mahaastami Barta"), // Ashwin
                    (6, 17, "2011-10-04", "Kaal Ratri"), // Ashwin
                    (6, 18, "2011-10-05", "Mahanawami"), // Ashwin
                    (6, 19, "2011-10-06", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 24, "2011-10-11", "Kojagratbrat"), // Ashwin
                    (7, 9, "2011-10-26", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 10, "2011-10-27", "Mha Pooja"), // Kartik
                    (7, 11, "2011-10-28", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 15, "2011-11-01", "Chhath Parba"), // Kartik
                    (7, 24, "2011-11-10", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (9, 10, "2011-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2011-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2012-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (10, 1, "2012-01-15", "Maghe Sankranti "), // Magh
                    (10, 10, "2012-01-24", "Sonam (Tamang) Lhosar"), // Magh
                    (10, 16, "2012-01-30", "Martyrs' Day"), // Magh
                    (11, 7, "2012-02-19", "National Democracy Day"), // Falgun
                    (11, 8, "2012-02-20", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 10, "2012-02-22", "Gyalpo Lhosar"), // Falgun
                    (11, 24, "2012-03-07", "Fagu Purnima"), // Falgun
                    (12, 18, "2012-03-31", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 22, "2012-04-04", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2070, new List<(int, int, string, string)>
                {
                    (1, 6, "2013-04-19", "Raam Nawami Brat (Raam Jayanti)"), // Baisakh
                    (1, 10, "2013-04-23", "Mahabir Jayanti (Jain only)"), // Baisakh
                    (1, 11, "2013-04-24", "Loktantra Diwas"), // Baisakh
                    (2, 11, "2013-05-25", "Buddha Jayanti"), // Jestha
                    (4, 29, "2013-08-13", "Udhauli Parwa"), // Shrawan
                    (5, 5, "2013-08-21", "Rishitarpani (Janai Purnima)"), // Bhadra
                    (5, 12, "2013-08-28", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 22, "2013-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (5, 23, "2013-09-08", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 25, "2013-09-10", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (6, 2, "2013-09-18", "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Ashwin
                    (6, 19, "2013-10-05", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 25, "2013-10-11", "Phulpati"), // Ashwin
                    (6, 26, "2013-10-12", "Mahaastami Barta"), // Ashwin
                    (6, 26, "2013-10-12", "Kaal Ratri"), // Ashwin
                    (6, 27, "2013-10-13", "Mahanawami"), // Ashwin
                    (6, 28, "2013-10-14", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 1, "2013-10-18", "Kojagratbrat"), // Kartik
                    (7, 17, "2013-11-03", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 18, "2013-11-04", "Mha Pooja"), // Kartik
                    (7, 19, "2013-11-05", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 23, "2013-11-09", "Chhath Parba"), // Kartik
                    (7, 25, "2013-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (8, 18, "2013-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 2, "2013-12-17", "Udhauli Parwa"), // Poush
                    (9, 10, "2013-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2013-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2014-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (10, 1, "2014-01-15", "Maghe Sankranti "), // Magh
                    (10, 16, "2014-01-30", "Martyrs' Day"), // Magh
                    (10, 17, "2014-01-31", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2014-02-19", "National Democracy Day"), // Falgun
                    (11, 15, "2014-02-27", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 18, "2014-03-02", "Gyalpo Lhosar"), // Falgun
                    (12, 2, "2014-03-16", "Fagu Purnima"), // Chaitra
                    (12, 5, "2014-03-19", "Udhauli Parwa"), // Chaitra
                    (12, 25, "2014-04-08", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 30, "2014-04-13", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2071, new List<(int, int, string, string)>
                {
                    (1, 11, "2014-04-24", "Loktantra Diwas"), // Baisakh
                    (1, 31, "2014-05-14", "Buddha Jayanti"), // Baisakh
                    (2, 15, "2014-05-29", "Republic Day"), // Jestha
                    (4, 25, "2014-08-10", "Rishitarpani (Janai Purnima)"), // Shrawan
                    (5, 1, "2014-08-17", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 12, "2014-08-28", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 14, "2014-08-30", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 22, "2014-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (5, 23, "2014-09-08",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (6, 9, "2014-09-25", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 15, "2014-10-01", "Phulpati"), // Ashwin
                    (6, 16, "2014-10-02", "Mahaastami Barta"), // Ashwin
                    (6, 16, "2014-10-02", "Kaal Ratri"), // Ashwin
                    (6, 17, "2014-10-03", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 6, "2014-10-23", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 7, "2014-10-24", "Mha Pooja"), // Kartik
                    (7, 8, "2014-10-25", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 12, "2014-10-29", "Chhath Parba"), // Kartik
                    (7, 25, "2014-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (8, 17, "2014-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 10, "2014-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2014-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2015-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (10, 1, "2015-01-15", "Maghe Sankranti "), // Magh
                    (10, 1, "2015-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 7, "2015-01-21", "Sonam (Tamang) Lhosar"), // Magh
                    (10, 16, "2015-01-30", "Martyrs' Day"), // Magh
                    (11, 5, "2015-02-17", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 7, "2015-02-19", "National Democracy Day"), // Falgun
                    (11, 7, "2015-02-19", "Gyalpo Lhosar"), // Falgun
                    (11, 21, "2015-03-05", "Fagu Purnima"), // Falgun
                    (12, 14, "2015-03-28", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 18, "2015-04-01", "Mahabir Jayanti (Jain only)"), // Chaitra
                    (12, 19, "2015-04-02", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2072, new List<(int, int, string, string)>
                {
                    (1, 11, "2015-04-24", "Loktantra Diwas"), // Baisakh
                    (1, 21, "2015-05-04", "Buddha Jayanti"), // Baisakh
                    (2, 15, "2015-05-29", "Republic Day"), // Jestha
                    (5, 12, "2015-08-29", "Rishitarpani (Janai Purnima)"), // Bhadra
                    (5, 19, "2015-09-05", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 22, "2015-09-08", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (5, 30, "2015-09-16", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (6, 1, "2015-09-18", "Rishi Panchami (Saptarishi Puja)"), // Ashwin
                    (6, 10, "2015-09-27",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Ashwin
                    (6, 26, "2015-10-13", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (7, 3, "2015-10-20", "Phulpati"), // Kartik
                    (7, 4, "2015-10-21", "Mahaastami Barta"), // Kartik
                    (7, 4, "2015-10-21", "Kaal Ratri"), // Kartik
                    (7, 5, "2015-10-22", "Mahanawami"), // Kartik
                    (7, 5, "2015-10-22", "Vijaya Dashami (Dashainko Tika)"), // Kartik
                    (7, 25, "2015-11-11", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 26, "2015-11-12", "Mha Pooja"), // Kartik
                    (7, 27, "2015-11-13", "Bhai Tika (Kija Puja)"), // Kartik
                    (8, 1, "2015-11-17", "Chhath Parba"), // Mangsir
                    (8, 17, "2015-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 10, "2015-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2015-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2016-01-11", "Rastriya Ekata Diwas"), // Poush
                    (10, 1, "2016-01-15", "Maghe Sankranti "), // Magh
                    (10, 1, "2016-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 26, "2016-02-09", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2016-02-19", "National Democracy Day"), // Falgun
                    (11, 24, "2016-03-07", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 26, "2016-03-09", "Gyalpo Lhosar"), // Falgun
                    (12, 9, "2016-03-22", "Fagu Purnima") // Chaitra
                }
            },
            {
                2073, new List<(int, int, string, string)>
                {
                    (1, 3, "2016-04-15", "Raam Nawami Brat (Raam Jayanti)"), // Baisakh
                    (1, 7, "2016-04-19", "Mahabir Jayanti (Jain only)"), // Baisakh
                    (1, 11, "2016-04-23", "Loktantra Diwas"), // Baisakh
                    (2, 8, "2016-05-21", "Buddha Jayanti"), // Jestha
                    (2, 15, "2016-05-28", "Republic Day"), // Jestha
                    (5, 2, "2016-08-18", "Rishitarpani (Janai Purnima)"), // Bhadra
                    (5, 9, "2016-08-25", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 19, "2016-09-04", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 21, "2016-09-06", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 22, "2016-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (5, 30, "2016-09-15",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (6, 3, "2016-09-19", "Constitution Day"), // Ashwin
                    (6, 15, "2016-10-01", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 22, "2016-10-08", "Phulpati"), // Ashwin
                    (6, 23, "2016-10-09", "Mahaastami Barta"), // Ashwin
                    (6, 23, "2016-10-09", "Kaal Ratri"), // Ashwin
                    (6, 24, "2016-10-10", "Mahanawami"), // Ashwin
                    (6, 25, "2016-10-11", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 29, "2016-10-15", "Kojagratbrat"), // Ashwin
                    (7, 14, "2016-10-30", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 15, "2016-10-31", "Mha Pooja"), // Kartik
                    (7, 16, "2016-11-01", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 21, "2016-11-06", "Chhath Parba"), // Kartik
                    (7, 25, "2016-11-10", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 28, "2016-11-13", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (8, 18, "2016-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (8, 28, "2016-12-13", "Udhauli Parwa"), // Mangsir
                    (9, 10, "2016-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2016-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2017-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (9, 27, "2017-01-11", "Rastriya Ekata Diwas"), // Poush
                    (10, 1, "2017-01-14", "Maghe Sankranti "), // Magh
                    (10, 1, "2017-01-14", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 15, "2017-01-28", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2017-02-18", "National Democracy Day"), // Falgun
                    (11, 13, "2017-02-24", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 16, "2017-02-27", "Gyalpo Lhosar"), // Falgun
                    (11, 29, "2017-03-12", "Fagu Purnima"), // Falgun
                    (12, 23, "2017-04-05", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 27, "2017-04-09", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2074, new List<(int, int, string, string)>
                {
                    (1, 11, "2017-04-24", "Loktantra Diwas"), // Baisakh
                    (1, 27, "2017-05-10", "Buddha Jayanti"), // Baisakh
                    (2, 15, "2017-05-29", "Republic Day"), // Jestha
                    (4, 30, "2017-08-14", "Shree Krishna Janmasthami Brata (Moharatri)"), // Shrawan
                    (5, 8, "2017-08-24", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 10, "2017-08-26", "Rishi Panchami (Saptarishi Puja)"), // Bhadra
                    (5, 20, "2017-09-05",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (5, 22, "2017-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 3, "2017-09-19", "Constitution Day"), // Ashwin
                    (6, 5, "2017-09-21", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 11, "2017-09-27", "Phulpati"), // Ashwin
                    (6, 12, "2017-09-28", "Mahaastami Barta"), // Ashwin
                    (6, 12, "2017-09-28", "Kaal Ratri"), // Ashwin
                    (6, 13, "2017-09-29", "Mahanawami"), // Ashwin
                    (6, 14, "2017-09-30", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 19, "2017-10-05", "Kojagratbrat"), // Ashwin
                    (7, 2, "2017-10-19", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 3, "2017-10-20", "Mha Pooja"), // Kartik
                    (7, 4, "2017-10-21", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 9, "2017-10-26", "Chhath Parba"), // Kartik
                    (7, 18, "2017-11-04", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (7, 25, "2017-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (8, 17, "2017-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (8, 17, "2017-12-03", "Udhauli Parwa"), // Mangsir
                    (9, 10, "2017-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2017-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2018-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (9, 27, "2018-01-11", "Rastriya Ekata Diwas"), // Poush
                    (10, 1, "2018-01-15", "Maghe Sankranti "), // Magh
                    (10, 1, "2018-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 4, "2018-01-18", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 1, "2018-02-13", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 4, "2018-02-16", "Gyalpo Lhosar"), // Falgun
                    (11, 7, "2018-02-19", "National Democracy Day"), // Falgun
                    (11, 17, "2018-03-01", "Fagu Purnima"), // Falgun
                    (12, 11, "2018-03-25", "Raam Nawami Brat (Raam Jayanti)"), // Chaitra
                    (12, 15, "2018-03-29", "Mahabir Jayanti (Jain only)") // Chaitra
                }
            },
            {
                2075, new List<(int, int, string, string)>
                {
                    (1, 17, "2018-04-30", "Buddha Jayanti"), // Baisakh
                    (5, 27, "2018-09-12", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (6, 3, "2018-09-19", "Constitution Day"), // Ashwin
                    (6, 8, "2018-09-24", "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Ashwin
                    (6, 16, "2018-10-02", "Jitiya Parwa (Women celebrating Jitiya Parwa Only)"), // Ashwin
                    (6, 30, "2018-10-16", "Phulpati"), // Ashwin
                    (6, 31, "2018-10-17", "Mahaastami Barta"), // Ashwin
                    (7, 1, "2018-10-18", "Mahanawami"), // Kartik
                    (7, 2, "2018-10-19", "Vijaya Dashami (Dashainko Tika)"), // Kartik
                    (7, 21, "2018-11-07", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 22, "2018-11-08", "Mha Pooja"), // Kartik
                    (7, 23, "2018-11-09", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 25, "2018-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 27, "2018-11-13", "Chhath Parba"), // Kartik
                    (8, 7, "2018-11-23", "Guru Nanak Jayanti (Sikh only)"), // Mangsir
                    (8, 17, "2018-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 10, "2018-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2018-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (10, 1, "2019-01-15", "Maghe Sankranti "), // Magh
                    (10, 22, "2019-02-05", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 20, "2019-03-04", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 23, "2019-03-07", "Gyalpo Lhosar"), // Falgun
                    (12, 6, "2019-03-20", "Fagu Purnima"), // Chaitra
                    (12, 22, "2019-04-05", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            },
            {
                2076, new List<(int, int, string, string)>
                {
                    (2, 4, "2019-05-18", "Buddha Jayanti"), // Jestha
                    (5, 16, "2019-09-02", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 27, "2019-09-13",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (6, 3, "2019-09-20", "Constitution Day"), // Ashwin
                    (6, 5, "2019-09-22", "Jitiya Parwa (Women celebrating Jitiya Parwa Only)"), // Ashwin
                    (6, 18, "2019-10-05", "Phulpati"), // Ashwin
                    (6, 19, "2019-10-06", "Mahaastami Barta"), // Ashwin
                    (6, 20, "2019-10-07", "Mahanawami"), // Ashwin
                    (6, 21, "2019-10-08", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 10, "2019-10-27", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 11, "2019-10-28", "Mha Pooja"), // Kartik
                    (7, 12, "2019-10-29", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 16, "2019-11-02", "Chhath Parba"), // Kartik
                    (7, 25, "2019-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 26, "2019-11-12", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (8, 17, "2019-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (8, 26, "2019-12-12", "Udhauli Parwa"), // Mangsir
                    (9, 9, "2019-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2019-12-31", "Tamu (Gurung) Lhosar"), // Poush
                    (10, 1, "2020-01-15", "Maghe Sankranti "), // Magh
                    (10, 11, "2020-01-25", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 9, "2020-02-21", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 12, "2020-02-24", "Gyalpo Lhosar"), // Falgun
                    (11, 26, "2020-03-09", "Fagu Purnima"), // Falgun
                    (12, 11, "2020-03-24", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            },
            {
                2077, new List<(int, int, string, string)>
                {
                    (1, 25, "2020-05-07", "Ubhauli Parba"), // Baisakh
                    (1, 25, "2020-05-07", "Buddha Jayanti"), // Baisakh
                    (2, 12, "2020-05-25", "Eid-al-Fitr - Eid (Muslim only) (date may vary)"), // Jestha
                    (5, 5, "2020-08-21", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 10, "2020-08-26", "Gaura Parwa (Related Field only)"), // Bhadra
                    (5, 16, "2020-09-01",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (5, 25, "2020-09-10", "Jitiya Parwa (Women celebrating Jitiya Parwa Only)"), // Bhadra
                    (5, 25, "2020-09-10", "Jiwatputrika Brat(Jitiya Parwa)"), // Bhadra
                    (6, 3, "2020-09-19", "Constitution Day"), // Ashwin
                    (7, 7, "2020-10-23", "Phulpati"), // Kartik
                    (7, 8, "2020-10-24", "Kaal Ratri"), // Kartik
                    (7, 9, "2020-10-25", "Mahanawami"), // Kartik
                    (7, 10, "2020-10-26", "Vijaya Dashami (Dashainko Tika)"), // Kartik
                    (7, 10, "2020-10-26", "Devivisarjan "), // Kartik
                    (7, 25, "2020-11-10", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 29, "2020-11-14", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 30, "2020-11-15", "Mha Pooja"), // Kartik
                    (8, 1, "2020-11-16", "Bhai Tika (Kija Puja)"), // Mangsir
                    (8, 1, "2020-11-16", "Gobardhan Pooja"), // Mangsir
                    (8, 5, "2020-11-20", "Chhath Parba"), // Mangsir
                    (8, 15, "2020-11-30", "Guru Nanak Jayanti (Sikh only)"), // Mangsir
                    (8, 18, "2020-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 10, "2020-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2020-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 15, "2020-12-30", "Udhauli Parwa"), // Poush
                    (10, 1, "2021-01-14", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 1, "2021-01-14", "Maghe Sankranti "), // Magh
                    (10, 2, "2021-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 30, "2021-02-12", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 27, "2021-03-11", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (12, 1, "2021-03-14", "Gyalpo Lhosar"), // Chaitra
                    (12, 15, "2021-03-28", "Fagu Purnima"), // Chaitra
                    (12, 16, "2021-03-29", "Terai Holi"), // Chaitra
                    (12, 29, "2021-04-11", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            },
            {
                2078, new List<(int, int, string, string)>
                {
                    (1, 12, "2021-04-25", "Mahabir Jayanti (Jain only)"), // Baisakh
                    (2, 12, "2021-05-26", "Buddha Jayanti"), // Jestha
                    (2, 12, "2021-05-26", "Ubhauli Parba"), // Jestha
                    (2, 15, "2021-05-29", "Republic Day"), // Jestha
                    (5, 7, "2021-08-23", "Goyaatraa (Gaaiijaatraa) Saapaaru"), // Bhadra
                    (5, 24, "2021-09-09", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (6, 3, "2021-09-19", "Constitution Day"), // Ashwin
                    (6, 21, "2021-10-07", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 26, "2021-10-12", "Phulpati"), // Ashwin
                    (6, 27, "2021-10-13", "Mahaastami Barta"), // Ashwin
                    (6, 28, "2021-10-14", "Mahanawami"), // Ashwin
                    (6, 29, "2021-10-15", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (7, 18, "2021-11-04", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 19, "2021-11-05", "Gobardhan Pooja"), // Kartik
                    (7, 20, "2021-11-06", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 24, "2021-11-10", "Chhath Parba"), // Kartik
                    (7, 25, "2021-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (8, 3, "2021-11-19", "Guru Nanak Jayanti (Sikh only)"), // Mangsir
                    (8, 17, "2021-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 4, "2021-12-19", "Yo:mari Punhi"), // Poush
                    (9, 4, "2021-12-19", "Udhauli Parwa"), // Poush
                    (9, 4, "2021-12-19", "Dhanya Purnima"), // Poush
                    (9, 15, "2021-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (10, 1, "2022-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 1, "2022-01-15", "Maghe Sankranti "), // Magh
                    (10, 19, "2022-02-02", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2022-02-19", "National Democracy Day"), // Falgun
                    (11, 17, "2022-03-01", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 17, "2022-03-01", "Maha Shivaratri"), // Falgun
                    (12, 3, "2022-03-17", "Fagu Purnima"), // Chaitra
                    (12, 4, "2022-03-18", "Terai Holi"), // Chaitra
                    (12, 18, "2022-04-01", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            },
            {
                2079, new List<(int, int, string, string)>
                {
                    (2, 1, "2022-05-15", "Public Holiday"), // Jestha
                    (2, 2, "2022-05-16", "Ubhauli Parba"), // Jestha
                    (2, 2, "2022-05-16", "Buddha Jayanti"), // Jestha
                    (2, 8, "2022-05-22", "Public Holiday"), // Jestha
                    (2, 15, "2022-05-29", "Republic Day"), // Jestha
                    (2, 15, "2022-05-29", "Public Holiday"), // Jestha
                    (2, 22, "2022-06-05", "Public Holiday"), // Jestha
                    (2, 29, "2022-06-12", "Public Holiday"), // Jestha
                    (4, 27, "2022-08-12", "Rishitarpani (Janai Purnima)"), // Shrawan
                    (4, 27, "2022-08-12", "Goyaatraa (Gaaiijaatraa) Saapaaru"), // Shrawan
                    (5, 3, "2022-08-19", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 24, "2022-09-09",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Bhadra
                    (6, 3, "2022-09-19", "Constitution Day"), // Ashwin
                    (6, 10, "2022-09-26", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 16, "2022-10-02", "Phulpati"), // Ashwin
                    (6, 17, "2022-10-03", "Mahaastami Barta"), // Ashwin
                    (6, 18, "2022-10-04", "Mahanawami"), // Ashwin
                    (6, 19, "2022-10-05", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 21, "2022-10-07", "Government Day"), // Ashwin
                    (7, 7, "2022-10-24", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 8, "2022-10-25", "Gai Tihar"), // Kartik
                    (7, 9, "2022-10-26", "Gobardhan Pooja"), // Kartik
                    (7, 9, "2022-10-26", "Mha Pooja"), // Kartik
                    (7, 10, "2022-10-27", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 13, "2022-10-30", "Chhath Parba"), // Kartik
                    (7, 22, "2022-11-08", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (8, 22, "2022-12-08", "Udhauli Parwa"), // Mangsir
                    (8, 22, "2022-12-08", "Yo:mari Punhi"), // Mangsir
                    (9, 10, "2022-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2022-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2023-01-11", "Rastriya Ekata Diwas"), // Poush
                    (10, 8, "2023-01-22", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 6, "2023-02-18", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 7, "2023-02-19", "National Democracy Day"), // Falgun
                    (11, 9, "2023-02-21", "Gyalpo Lhosar"), // Falgun
                    (11, 23, "2023-03-07", "Terai Holi"), // Falgun
                    (11, 24, "2023-03-08", "International Women's Day"), // Falgun
                    (12, 7, "2023-03-21", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            },
            {
                2080, new List<(int, int, string, string)>
                {
                    (1, 18, "2023-05-01", "International Labour Day"), // Baisakh
                    (1, 22, "2023-05-05", "Ubhauli Parba"), // Baisakh
                    (1, 22, "2023-05-05", "Chandi Purnima"), // Baisakh
                    (1, 22, "2023-05-05", "Buddha Jayanti"), // Baisakh
                    (2, 11, "2023-05-25", "Bhoto Jatra (KTM valley only)"), // Jestha
                    (2, 15, "2023-05-29", "Republic Day"), // Jestha
                    (5, 14, "2023-08-31", "Goyaatraa (Gaaiijaatraa) Saapaaru"), // Bhadra
                    (5, 20, "2023-09-06", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (6, 1, "2023-09-18", "Haritalika Teej Brata (Women only)"), // Ashwin
                    (6, 3, "2023-09-20", "Constitution Day"), // Ashwin
                    (6, 11, "2023-09-28",
                        "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Ashwin
                    (6, 20, "2023-10-07", "Jiwatputrika Brat(Jitiya Parwa)"), // Ashwin
                    (6, 28, "2023-10-15", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (7, 4, "2023-10-21", "Phulpati"), // Kartik
                    (7, 5, "2023-10-22", "Mahaastami Barta"), // Kartik
                    (7, 6, "2023-10-23", "Mahanawami"), // Kartik
                    (7, 7, "2023-10-24", "Vijaya Dashami (Dashainko Tika)"), // Kartik
                    (7, 8, "2023-10-25", "Government Holiday"), // Kartik
                    (7, 9, "2023-10-26", "Government Day"), // Kartik
                    (7, 9, "2023-10-26", "Dashain Holiday"), // Kartik
                    (7, 25, "2023-11-11", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 26, "2023-11-12", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 27, "2023-11-13", "Government Holiday"), // Kartik
                    (7, 28, "2023-11-14", "Mha Pooja"), // Kartik
                    (7, 28, "2023-11-14", "Gobardhan Pooja"), // Kartik
                    (7, 29, "2023-11-15", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 30, "2023-11-16", "Tihar Holiday"), // Kartik
                    (8, 3, "2023-11-19", "Chhath Parba"), // Mangsir
                    (8, 11, "2023-11-27", "Guru Nanak Jayanti (Sikh only)"), // Mangsir
                    (8, 17, "2023-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (9, 9, "2023-12-25", "Christmas (Christian only)"), // Poush
                    (9, 10, "2023-12-26", "Udhauli Parwa"), // Poush
                    (9, 10, "2023-12-26", "Yo:mari Punhi"), // Poush
                    (9, 10, "2023-12-26", "Dhanya Purnima"), // Poush
                    (9, 15, "2023-12-31", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2024-01-12", "Prithivi Jayanti, National Unity Day"), // Poush
                    (9, 27, "2024-01-12", "Rastriya Ekata Diwas"), // Poush
                    (10, 1, "2024-01-15", "Maghe Sankranti"), // Magh
                    (10, 1, "2024-01-15", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 27, "2024-02-10", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2024-02-19", "National Democracy Day"), // Falgun
                    (11, 25, "2024-03-08", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 25, "2024-03-08", "International Women's Day"), // Falgun
                    (11, 28, "2024-03-11", "Gyalpo Lhosar"), // Falgun
                    (12, 11, "2024-03-24", "Pahadma Holi"), // Chaitra
                    (12, 12, "2024-03-25", "Terai Holi"), // Chaitra
                    (12, 12, "2024-03-25", "Fagu Purnima"), // Chaitra
                    (12, 26, "2024-04-08", "Ghode Jatra (KTM valley only)"), // Chaitra
                    (12, 29, "2024-04-11", "Eid-ul-Fitr - Eid (Muslim only)") // Chaitra
                }
            },
            {
                2081, new List<(int, int, string, string)>
                {
                    (1, 5, "2024-04-17", "Raam Nawami Brat (Raam Jayanti)"), // Baisakh
                    (1, 19, "2024-05-01", "International Labour Day"), // Baisakh
                    (2, 10, "2024-05-23", "Chandi Purnima"), // Jestha
                    (2, 10, "2024-05-23", "Ubhauli Parba"), // Jestha
                    (2, 10, "2024-05-23", "Buddha Jayanti"), // Jestha
                    (2, 15, "2024-05-28", "Republic Day"), // Jestha
                    (3, 3, "2024-06-17", "Eid-Al-Adha (Muslim only)"), // Ashad
                    (4, 20, "2024-08-04", "Bhoto Jaatra (Kathmandu Valley only)"), // Shrawan
                    (5, 3, "2024-08-19", "Raksha Bandhan"), // Bhadra
                    (5, 4, "2024-08-20", "Goyaatraa (Gaaiijaatraa) Saapaaru"), // Bhadra
                    (5, 10, "2024-08-26", "Shree Krishna Janmasthami Brata (Moharatri)"), // Bhadra
                    (5, 10, "2024-08-26", "Gaura Parwa (Related Field only)"), // Bhadra
                    (5, 21, "2024-09-06", "Haritalika Teej Brata (Women only)"), // Bhadra
                    (5, 22, "2024-09-07", "Civil Service Day (Civil Employees Only)"), // Bhadra
                    (6, 1, "2024-09-17", "Indrajatra(Swachyaa) (Kumari-Bhairab Ganesh ko Raath Jaatraa)"), // Ashwin
                    (6, 3, "2024-09-19", "Constitution Day"), // Ashwin
                    (6, 9, "2024-09-25", "Jiwatputrika Brat(Jitiya Parwa)"), // Ashwin
                    (6, 17, "2024-10-03", "Ghatasthapana (Na:laa Swone)"), // Ashwin
                    (6, 24, "2024-10-10", "Nawa Patrika Prawes (Fulpati)"), // Ashwin
                    (6, 25, "2024-10-11", "Mahaastami Barta"), // Ashwin
                    (6, 25, "2024-10-11", "Mahanawami"), // Ashwin
                    (6, 26, "2024-10-12", "Vijaya Dashami (Dashainko Tika)"), // Ashwin
                    (6, 27, "2024-10-13", "Dashain Holiday"), // Ashwin
                    (6, 28, "2024-10-14", "Dashain Holiday"), // Ashwin
                    (7, 15, "2024-10-31", "Dipawali (Laxmi Puja)"), // Kartik
                    (7, 16, "2024-11-01", "Gai Tihar-Puja"), // Kartik
                    (7, 17, "2024-11-02", "Mha Pooja"), // Kartik
                    (7, 17, "2024-11-02", "Nepal Sambat 1145 starts"), // Kartik
                    (7, 17, "2024-11-02", "Gobardhan Pooja"), // Kartik
                    (7, 18, "2024-11-03", "Bhai Tika (Kija Puja)"), // Kartik
                    (7, 22, "2024-11-07", "Chhath Parba"), // Kartik
                    (7, 25, "2024-11-10", "Falgunanda Jayanti (Kirat only)"), // Kartik
                    (7, 30, "2024-11-15", "Guru Nanak Jayanti (Sikh only)"), // Kartik
                    (8, 18, "2024-12-03",
                        "International Day of Persons with Disabilities (Differently abled only)"), // Mangsir
                    (8, 30, "2024-12-15", "Dhanya Purnima"), // Mangsir
                    (8, 30, "2024-12-15", "Udhauli Parwa"), // Mangsir
                    (8, 30, "2024-12-15", "Yo:mari Punhi"), // Mangsir
                    (9, 10, "2024-12-25", "Christmas (Christian only)"), // Poush
                    (9, 15, "2024-12-30", "Tamu (Gurung) Lhosar"), // Poush
                    (9, 27, "2025-01-11", "Prithivi Jayanti, National Unity Day"), // Poush
                    (10, 1, "2025-01-14", "Maghi Parwa(Tharu, Magar, chhantyaalaadiko parba)"), // Magh
                    (10, 1, "2025-01-14", "Maghe Sankranti"), // Magh
                    (10, 16, "2025-01-29", "Martyrs' Day"), // Magh
                    (10, 17, "2025-01-30", "Sonam (Tamang) Lhosar"), // Magh
                    (11, 7, "2025-02-19", "National Democracy Day"), // Falgun
                    (11, 14, "2025-02-26", "Maha Shivaratri Chaturdasi Brat"), // Falgun
                    (11, 16, "2025-02-28", "Gyalpo (sherpa) Lhosar"), // Falgun
                    (11, 16, "2025-02-28", "Gyalpo Lhosar"), // Falgun
                    (11, 24, "2025-03-08", "International Women's Day"), // Falgun
                    (11, 29, "2025-03-13", "Pahadma Holi"), // Falgun
                    (12, 1, "2025-03-14", "Fagu Purnima"), // Chaitra
                    (12, 16, "2025-03-29", "Ghode Jatra (KTM valley only)") // Chaitra
                }
            }
        };
}