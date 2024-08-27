declare @temp_timezones table
(
    id int, 
    name NVARCHAR(255), 
    offset NVARCHAR(10),
    description NVARCHAR(255)
)

insert into @temp_timezones(id, name, description, offset) values(1,'Dateline Standard Time','(UTC-12:00) International Date Line West','-12:00');
insert into @temp_timezones(id, name, description, offset) values(2,'UTC-11','(UTC-11:00) Coordinated Universal Time-11','-11:00');
insert into @temp_timezones(id, name, description, offset) values(3,'Aleutian Standard Time','(UTC-10:00) Aleutian Islands','-09:00');
insert into @temp_timezones(id, name, description, offset) values(4,'Hawaiian Standard Time','(UTC-10:00) Hawaii','-10:00');
insert into @temp_timezones(id, name, description, offset) values(5,'Marquesas Standard Time','(UTC-09:30) Marquesas Islands','-09:30');
insert into @temp_timezones(id, name, description, offset) values(6,'Alaskan Standard Time','(UTC-09:00) Alaska','-08:00');
insert into @temp_timezones(id, name, description, offset) values(7,'UTC-09','(UTC-09:00) Coordinated Universal Time-09','-09:00');
insert into @temp_timezones(id, name, description, offset) values(8,'Pacific Standard Time (Mexico)','(UTC-08:00) Baja California','-07:00');
insert into @temp_timezones(id, name, description, offset) values(9,'UTC-08','(UTC-08:00) Coordinated Universal Time-08','-08:00');
insert into @temp_timezones(id, name, description, offset) values(10,'Pacific Standard Time','(UTC-08:00) Pacific Time (US & Canada)','-07:00');
insert into @temp_timezones(id, name, description, offset) values(11,'US Mountain Standard Time','(UTC-07:00) Arizona','-07:00');
insert into @temp_timezones(id, name, description, offset) values(12,'Mountain Standard Time (Mexico)','(UTC-07:00) Chihuahua, La Paz, Mazatlan','-06:00');
insert into @temp_timezones(id, name, description, offset) values(13,'Mountain Standard Time','(UTC-07:00) Mountain Time (US & Canada)','-06:00');
insert into @temp_timezones(id, name, description, offset) values(14,'Yukon Standard Time','(UTC-07:00) Yukon','-07:00');
insert into @temp_timezones(id, name, description, offset) values(15,'Central America Standard Time','(UTC-06:00) Central America','-06:00');
insert into @temp_timezones(id, name, description, offset) values(16,'Central Standard Time','(UTC-06:00) Central Time (US & Canada)','-05:00');
insert into @temp_timezones(id, name, description, offset) values(17,'Easter Island Standard Time','(UTC-06:00) Easter Island','-06:00');
insert into @temp_timezones(id, name, description, offset) values(18,'Central Standard Time (Mexico)','(UTC-06:00) Guadalajara, Mexico City, Monterrey','-05:00');
insert into @temp_timezones(id, name, description, offset) values(19,'Canada Central Standard Time','(UTC-06:00) Saskatchewan','-06:00');
insert into @temp_timezones(id, name, description, offset) values(20,'SA Pacific Standard Time','(UTC-05:00) Bogota, Lima, Quito, Rio Branco','-05:00');
insert into @temp_timezones(id, name, description, offset) values(21,'Eastern Standard Time (Mexico)','(UTC-05:00) Chetumal','-05:00');
insert into @temp_timezones(id, name, description, offset) values(22,'Eastern Standard Time','(UTC-05:00) Eastern Time (US & Canada)','-04:00');
insert into @temp_timezones(id, name, description, offset) values(23,'Haiti Standard Time','(UTC-05:00) Haiti','-04:00');
insert into @temp_timezones(id, name, description, offset) values(24,'Cuba Standard Time','(UTC-05:00) Havana','-04:00');
insert into @temp_timezones(id, name, description, offset) values(25,'US Eastern Standard Time','(UTC-05:00) Indiana (East)','-04:00');
insert into @temp_timezones(id, name, description, offset) values(26,'Turks And Caicos Standard Time','(UTC-05:00) Turks and Caicos','-04:00');
insert into @temp_timezones(id, name, description, offset) values(27,'Paraguay Standard Time','(UTC-04:00) Asuncion','-04:00');
insert into @temp_timezones(id, name, description, offset) values(28,'Atlantic Standard Time','(UTC-04:00) Atlantic Time (Canada)','-03:00');
insert into @temp_timezones(id, name, description, offset) values(29,'Venezuela Standard Time','(UTC-04:00) Caracas','-04:00');
insert into @temp_timezones(id, name, description, offset) values(30,'Central Brazilian Standard Time','(UTC-04:00) Cuiaba','-04:00');
insert into @temp_timezones(id, name, description, offset) values(31,'SA Western Standard Time','(UTC-04:00) Georgetown, La Paz, Manaus, San Juan','-04:00');
insert into @temp_timezones(id, name, description, offset) values(32,'Pacific SA Standard Time','(UTC-04:00) Santiago','-04:00');
insert into @temp_timezones(id, name, description, offset) values(33,'Newfoundland Standard Time','(UTC-03:30) Newfoundland','-02:30');
insert into @temp_timezones(id, name, description, offset) values(34,'Tocantins Standard Time','(UTC-03:00) Araguaina','-03:00');
insert into @temp_timezones(id, name, description, offset) values(35,'E. South America Standard Time','(UTC-03:00) Brasilia','-03:00');
insert into @temp_timezones(id, name, description, offset) values(36,'SA Eastern Standard Time','(UTC-03:00) Cayenne, Fortaleza','-03:00');
insert into @temp_timezones(id, name, description, offset) values(37,'Argentina Standard Time','(UTC-03:00) City of Buenos Aires','-03:00');
insert into @temp_timezones(id, name, description, offset) values(38,'Greenland Standard Time','(UTC-03:00) Greenland','-03:00');
insert into @temp_timezones(id, name, description, offset) values(39,'Montevideo Standard Time','(UTC-03:00) Montevideo','-03:00');
insert into @temp_timezones(id, name, description, offset) values(40,'Magallanes Standard Time','(UTC-03:00) Punta Arenas','-03:00');
insert into @temp_timezones(id, name, description, offset) values(41,'Saint Pierre Standard Time','(UTC-03:00) Saint Pierre and Miquelon','-02:00');
insert into @temp_timezones(id, name, description, offset) values(42,'Bahia Standard Time','(UTC-03:00) Salvador','-03:00');
insert into @temp_timezones(id, name, description, offset) values(43,'UTC-02','(UTC-02:00) Coordinated Universal Time-02','-02:00');
insert into @temp_timezones(id, name, description, offset) values(44,'Mid-Atlantic Standard Time','(UTC-02:00) Mid-Atlantic - Old','-01:00');
insert into @temp_timezones(id, name, description, offset) values(45,'Azores Standard Time','(UTC-01:00) Azores','+00:00');
insert into @temp_timezones(id, name, description, offset) values(46,'Cape Verde Standard Time','(UTC-01:00) Cabo Verde Is.','-01:00');
insert into @temp_timezones(id, name, description, offset) values(47,'UTC','(UTC) Coordinated Universal Time','+00:00');
insert into @temp_timezones(id, name, description, offset) values(48,'GMT Standard Time','(UTC+00:00) Dublin, Edinburgh, Lisbon, London','+01:00');
insert into @temp_timezones(id, name, description, offset) values(49,'Greenwich Standard Time','(UTC+00:00) Monrovia, Reykjavik','+00:00');
insert into @temp_timezones(id, name, description, offset) values(50,'Sao Tome Standard Time','(UTC+00:00) Sao Tome','+00:00');
insert into @temp_timezones(id, name, description, offset) values(51,'Morocco Standard Time','(UTC+01:00) Casablanca','+01:00');
insert into @temp_timezones(id, name, description, offset) values(52,'W. Europe Standard Time','(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna','+02:00');
insert into @temp_timezones(id, name, description, offset) values(53,'Central Europe Standard Time','(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague','+02:00');
insert into @temp_timezones(id, name, description, offset) values(54,'Romance Standard Time','(UTC+01:00) Brussels, Copenhagen, Madrid, Paris','+02:00');
insert into @temp_timezones(id, name, description, offset) values(55,'Central European Standard Time','(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb','+02:00');
insert into @temp_timezones(id, name, description, offset) values(56,'W. Central Africa Standard Time','(UTC+01:00) West Central Africa','+01:00');
insert into @temp_timezones(id, name, description, offset) values(57,'Jordan Standard Time','(UTC+02:00) Amman','+03:00');
insert into @temp_timezones(id, name, description, offset) values(58,'GTB Standard Time','(UTC+02:00) Athens, Bucharest','+03:00');
insert into @temp_timezones(id, name, description, offset) values(59,'Middle East Standard Time','(UTC+02:00) Beirut','+03:00');
insert into @temp_timezones(id, name, description, offset) values(60,'Egypt Standard Time','(UTC+02:00) Cairo','+02:00');
insert into @temp_timezones(id, name, description, offset) values(61,'E. Europe Standard Time','(UTC+02:00) Chisinau','+03:00');
insert into @temp_timezones(id, name, description, offset) values(62,'Syria Standard Time','(UTC+02:00) Damascus','+03:00');
insert into @temp_timezones(id, name, description, offset) values(63,'West Bank Standard Time','(UTC+02:00) Gaza, Hebron','+03:00');
insert into @temp_timezones(id, name, description, offset) values(64,'South Africa Standard Time','(UTC+02:00) Harare, Pretoria','+02:00');
insert into @temp_timezones(id, name, description, offset) values(65,'FLE Standard Time','(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius','+03:00');
insert into @temp_timezones(id, name, description, offset) values(66,'Israel Standard Time','(UTC+02:00) Jerusalem','+03:00');
insert into @temp_timezones(id, name, description, offset) values(67,'South Sudan Standard Time','(UTC+02:00) Juba','+02:00');
insert into @temp_timezones(id, name, description, offset) values(68,'Kaliningrad Standard Time','(UTC+02:00) Kaliningrad','+02:00');
insert into @temp_timezones(id, name, description, offset) values(69,'Sudan Standard Time','(UTC+02:00) Khartoum','+02:00');
insert into @temp_timezones(id, name, description, offset) values(70,'Libya Standard Time','(UTC+02:00) Tripoli','+02:00');
insert into @temp_timezones(id, name, description, offset) values(71,'Namibia Standard Time','(UTC+02:00) Windhoek','+02:00');
insert into @temp_timezones(id, name, description, offset) values(72,'Arabic Standard Time','(UTC+03:00) Baghdad','+03:00');
insert into @temp_timezones(id, name, description, offset) values(73,'Turkey Standard Time','(UTC+03:00) Istanbul','+03:00');
insert into @temp_timezones(id, name, description, offset) values(74,'Arab Standard Time','(UTC+03:00) Kuwait, Riyadh','+03:00');
insert into @temp_timezones(id, name, description, offset) values(75,'Belarus Standard Time','(UTC+03:00) Minsk','+03:00');
insert into @temp_timezones(id, name, description, offset) values(76,'Russian Standard Time','(UTC+03:00) Moscow, St. Petersburg','+03:00');
insert into @temp_timezones(id, name, description, offset) values(77,'E. Africa Standard Time','(UTC+03:00) Nairobi','+03:00');
insert into @temp_timezones(id, name, description, offset) values(78,'Volgograd Standard Time','(UTC+03:00) Volgograd','+03:00');
insert into @temp_timezones(id, name, description, offset) values(79,'Iran Standard Time','(UTC+03:30) Tehran','+04:30');
insert into @temp_timezones(id, name, description, offset) values(80,'Arabian Standard Time','(UTC+04:00) Abu Dhabi, Muscat','+04:00');
insert into @temp_timezones(id, name, description, offset) values(81,'Astrakhan Standard Time','(UTC+04:00) Astrakhan, Ulyanovsk','+04:00');
insert into @temp_timezones(id, name, description, offset) values(82,'Azerbaijan Standard Time','(UTC+04:00) Baku','+04:00');
insert into @temp_timezones(id, name, description, offset) values(83,'Russia Time Zone 3','(UTC+04:00) Izhevsk, Samara','+04:00');
insert into @temp_timezones(id, name, description, offset) values(84,'Mauritius Standard Time','(UTC+04:00) Port Louis','+04:00');
insert into @temp_timezones(id, name, description, offset) values(85,'Saratov Standard Time','(UTC+04:00) Saratov','+04:00');
insert into @temp_timezones(id, name, description, offset) values(86,'Georgian Standard Time','(UTC+04:00) Tbilisi','+04:00');
insert into @temp_timezones(id, name, description, offset) values(87,'Caucasus Standard Time','(UTC+04:00) Yerevan','+04:00');
insert into @temp_timezones(id, name, description, offset) values(88,'Afghanistan Standard Time','(UTC+04:30) Kabul','+04:30');
insert into @temp_timezones(id, name, description, offset) values(89,'West Asia Standard Time','(UTC+05:00) Ashgabat, Tashkent','+05:00');
insert into @temp_timezones(id, name, description, offset) values(90,'Ekaterinburg Standard Time','(UTC+05:00) Ekaterinburg','+05:00');
insert into @temp_timezones(id, name, description, offset) values(91,'Pakistan Standard Time','(UTC+05:00) Islamabad, Karachi','+05:00');
insert into @temp_timezones(id, name, description, offset) values(92,'Qyzylorda Standard Time','(UTC+05:00) Qyzylorda','+05:00');
insert into @temp_timezones(id, name, description, offset) values(93,'India Standard Time','(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi','+05:30');
insert into @temp_timezones(id, name, description, offset) values(94,'Sri Lanka Standard Time','(UTC+05:30) Sri Jayawardenepura','+05:30');
insert into @temp_timezones(id, name, description, offset) values(95,'Nepal Standard Time','(UTC+05:45) Kathmandu','+05:45');
insert into @temp_timezones(id, name, description, offset) values(96,'Central Asia Standard Time','(UTC+06:00) Astana','+06:00');
insert into @temp_timezones(id, name, description, offset) values(97,'Bangladesh Standard Time','(UTC+06:00) Dhaka','+06:00');
insert into @temp_timezones(id, name, description, offset) values(98,'Omsk Standard Time','(UTC+06:00) Omsk','+06:00');
insert into @temp_timezones(id, name, description, offset) values(99,'Myanmar Standard Time','(UTC+06:30) Yangon (Rangoon)','+06:30');
insert into @temp_timezones(id, name, description, offset) values(100,'SE Asia Standard Time','(UTC+07:00) Bangkok, Hanoi, Jakarta','+07:00');
insert into @temp_timezones(id, name, description, offset) values(101,'Altai Standard Time','(UTC+07:00) Barnaul, Gorno-Altaysk','+07:00');
insert into @temp_timezones(id, name, description, offset) values(102,'W. Mongolia Standard Time','(UTC+07:00) Hovd','+07:00');
insert into @temp_timezones(id, name, description, offset) values(103,'North Asia Standard Time','(UTC+07:00) Krasnoyarsk','+07:00');
insert into @temp_timezones(id, name, description, offset) values(104,'N. Central Asia Standard Time','(UTC+07:00) Novosibirsk','+07:00');
insert into @temp_timezones(id, name, description, offset) values(105,'Tomsk Standard Time','(UTC+07:00) Tomsk','+07:00');
insert into @temp_timezones(id, name, description, offset) values(106,'China Standard Time','(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi','+08:00');
insert into @temp_timezones(id, name, description, offset) values(107,'North Asia East Standard Time','(UTC+08:00) Irkutsk','+08:00');
insert into @temp_timezones(id, name, description, offset) values(108,'Singapore Standard Time','(UTC+08:00) Kuala Lumpur, Singapore','+08:00');
insert into @temp_timezones(id, name, description, offset) values(109,'W. Australia Standard Time','(UTC+08:00) Perth','+08:00');
insert into @temp_timezones(id, name, description, offset) values(110,'Taipei Standard Time','(UTC+08:00) Taipei','+08:00');
insert into @temp_timezones(id, name, description, offset) values(111,'Ulaanbaatar Standard Time','(UTC+08:00) Ulaanbaatar','+08:00');
insert into @temp_timezones(id, name, description, offset) values(112,'Aus Central W. Standard Time','(UTC+08:45) Eucla','+08:45');
insert into @temp_timezones(id, name, description, offset) values(113,'Transbaikal Standard Time','(UTC+09:00) Chita','+09:00');
insert into @temp_timezones(id, name, description, offset) values(114,'Tokyo Standard Time','(UTC+09:00) Osaka, Sapporo, Tokyo','+09:00');
insert into @temp_timezones(id, name, description, offset) values(115,'North Korea Standard Time','(UTC+09:00) Pyongyang','+09:00');
insert into @temp_timezones(id, name, description, offset) values(116,'Korea Standard Time','(UTC+09:00) Seoul','+09:00');
insert into @temp_timezones(id, name, description, offset) values(117,'Yakutsk Standard Time','(UTC+09:00) Yakutsk','+09:00');
insert into @temp_timezones(id, name, description, offset) values(118,'Cen. Australia Standard Time','(UTC+09:30) Adelaide','+09:30');
insert into @temp_timezones(id, name, description, offset) values(119,'AUS Central Standard Time','(UTC+09:30) Darwin','+09:30');
insert into @temp_timezones(id, name, description, offset) values(120,'E. Australia Standard Time','(UTC+10:00) Brisbane','+10:00');
insert into @temp_timezones(id, name, description, offset) values(121,'AUS Eastern Standard Time','(UTC+10:00) Canberra, Melbourne, Sydney','+10:00');
insert into @temp_timezones(id, name, description, offset) values(122,'West Pacific Standard Time','(UTC+10:00) Guam, Port Moresby','+10:00');
insert into @temp_timezones(id, name, description, offset) values(123,'Tasmania Standard Time','(UTC+10:00) Hobart','+10:00');
insert into @temp_timezones(id, name, description, offset) values(124,'Vladivostok Standard Time','(UTC+10:00) Vladivostok','+10:00');
insert into @temp_timezones(id, name, description, offset) values(125,'Lord Howe Standard Time','(UTC+10:30) Lord Howe Island','+10:30');
insert into @temp_timezones(id, name, description, offset) values(126,'Bougainville Standard Time','(UTC+11:00) Bougainville Island','+11:00');
insert into @temp_timezones(id, name, description, offset) values(127,'Russia Time Zone 10','(UTC+11:00) Chokurdakh','+11:00');
insert into @temp_timezones(id, name, description, offset) values(128,'Magadan Standard Time','(UTC+11:00) Magadan','+11:00');
insert into @temp_timezones(id, name, description, offset) values(129,'Norfolk Standard Time','(UTC+11:00) Norfolk Island','+11:00');
insert into @temp_timezones(id, name, description, offset) values(130,'Sakhalin Standard Time','(UTC+11:00) Sakhalin','+11:00');
insert into @temp_timezones(id, name, description, offset) values(131,'Central Pacific Standard Time','(UTC+11:00) Solomon Is., New Caledonia','+11:00');
insert into @temp_timezones(id, name, description, offset) values(132,'Russia Time Zone 11','(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky','+12:00');
insert into @temp_timezones(id, name, description, offset) values(133,'New Zealand Standard Time','(UTC+12:00) Auckland, Wellington','+12:00');
insert into @temp_timezones(id, name, description, offset) values(134,'UTC+12','(UTC+12:00) Coordinated Universal Time+12','+12:00');
insert into @temp_timezones(id, name, description, offset) values(135,'Fiji Standard Time','(UTC+12:00) Fiji','+12:00');
insert into @temp_timezones(id, name, description, offset) values(136,'Kamchatka Standard Time','(UTC+12:00) Petropavlovsk-Kamchatsky - Old','+13:00');
insert into @temp_timezones(id, name, description, offset) values(137,'Chatham Islands Standard Time','(UTC+12:45) Chatham Islands','+12:45');
insert into @temp_timezones(id, name, description, offset) values(138,'UTC+13','(UTC+13:00) Coordinated Universal Time+13','+13:00');
insert into @temp_timezones(id, name, description, offset) values(139,'Tonga Standard Time','(UTC+13:00) Nuku''alofa','+13:00');
insert into @temp_timezones(id, name, description, offset) values(140,'Samoa Standard Time','(UTC+13:00) Samoa','+13:00');
insert into @temp_timezones(id, name, description, offset) values(141,'Line Islands Standard Time','(UTC+14:00) Kiritimati Island','+14:00');

merge timezones as currentTable
using ( select * from @temp_timezones ) as tempTable on tempTable.id = currentTable.id
when matched then
    update set
         currentTable.name = tempTable.name,
         currentTable.description = tempTable.description,
         currentTable.offset = tempTable.offset
when not matched then
     insert ( id, name, description, offset )
     values ( tempTable.id, tempTable.name, tempTable.description, tempTable.offset );
