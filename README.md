# ScheduleAPI
API to get schedule for groups. Also allow to get raw changes.

Main purpose — using in schedules project.
Instead of write getting schedule and changes algorithms on every platform, you can use this API to get all needed data.

# Address
Web API locates on:
<strong>http://uksivtscheduleapi.azurewebsites.net/</strong>

<i>All controller parameters must be add after address part.</i>

# Controllers
There is two types of controllers, selected by suffix:
  1.  <b>Day controllers</b> (".../api/day/...") — Get information for one (select by dayIndex) parameter.
  2.  <b>Week controllers</b> (".../api/week/...") — Get information for current week (include sunday).

Then, controllers devided by they purpose:
  1.  <b>ScheduleDataBase{Suffix}</b> — Connect to database with schedule and get information for it. 
      But it have some limits related to original schedule files typos and mistakes (wrong format, different symbols, etc.);
  2.  <b>Schedule{Suffix}Asset</b> — Instead of database connection and select queries, this controller use assets,
      located in project. Also, this method doesn't have limits unlike previous;
  
  3.  <b>Changes{Suffix}</b> — This controller have another purpose. It give information about changes to user.
  
# Parameters
Most part of controllers use 2 parameters:
  1. dayIndex — Index (N > -1 && N < 7) of needed day, to get schedule or changes.
     Doesnt present in Week controllers.
     Default value: 0;
  2. groupName — Name of group to get schedule.
     Default value: 19П-3.
                                       
DataBase controllers also use 1 additional parameter:
  1. selectUnsecure — Cause database have some wrong values (for example: lesson teachers or places), user can delete this values and select only precise ones. Anything else will be replaced by default values.
     Default value — false.

# Examples
  1. <b>http://uksivtscheduleapi.azurewebsites.net/api/day/scheduledayasset?dayIndex=0&groupName=19%D0%BF-3</b> — Return monday asset-based schedule for group '19П-3';
  2. <b>http://uksivtscheduleapi.azurewebsites.net/api/week/scheduleweekasset?groupName=20%D0%B7%D0%B8%D0%BE-2</b> — Return week asset-based schedule for group '20ЗИО-2';
  3. <b>http://uksivtscheduleapi.azurewebsites.net/api/week/scheduledatabaseweek?groupName=20%D0%9F-3&selectUnsecure=true</b> — Return week databased-schedule for group '20П-3' with unsecure values;
  4. <b>http://uksivtscheduleapi.azurewebsites.net/api/day/changesday?dayIndex=0&groupName=19%D0%9F-3</b> — Return monday changes for group '19П-3'.
