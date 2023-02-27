# ScheduleAPI
API to get schedule for groups. Also allow to get raw changes.

Main purpose — using in schedules project.
Instead of write getting schedule and changes algorithms on every platform, you can use this API to get all needed data.

AND, before we start: Change(-s) = Replacement(-s).
Remember it. This is a feature.

## Address
Web API locates on:
  1. <strong>https://uksivt.azurewebsites.net/</strong> (<strong>Preview</strong>);
  2. <strong>http://uksivt.info/</strong> (<strong>Stable</strong>).

<i>All controller parameters must be add after address part.</i>

## Controllers
Mainly there is two types of controllers, selected by postfix:
  1.  <b>Day controllers</b> (".../api/.../day") — Get information for one (select by dayIndex) parameter.
  2.  <b>Week controllers</b> (".../api/.../week") — Get information for current week (include sunday).

Then, main controllers devided by their purpose:
  1.  <b>Schedule</b> — Selects information about base schedule (without changes). 
      Works fast and stable. Assets updates every semester, when base schedule updates;
  3.  <b>Changes</b> — This controller have another purpose. It give information about changes to user. 
      Also send information about date of selected day, even if changes not found.

Also, in API you can get other information. Like groups list or general search with all-data. 
For this purpose, you'll have to use other group of controllers:
  1. <b>Structure</b> — Gets information about student structure (branches, affiliates, groups lists). 
     It contains four standalone getters:
     1. <b>Branches</b> — Returns information about branches (like "Programming" or "Computing Technologies");
     2. <b>Affiliates</b> — Returns information about affiliates on selected branch. Like "П" or "ВЕБ" for "Programming" branch.
        It requires string parameter: selected branch. Default value: "Programming";
     3. <b>Groups</b> — Returns information about groups by selected branch and affiliate.
        It requires two string parameters: branch and affiliate.
        Default values: "Programming" and "П";
     4. <b>Summary</b> — Returns full list of groups. Doesn't require any parameters.
        Executes little bit slower, than others.
  2. <b>Search</b> — Searches by setted criteria.
  
## Parameters
### Main controllers
Most part of controllers use 2 parameters:
  1. <i>dayIndex</i> — Index (N > -1 && N < 7) of needed day, to get schedule or changes.
     Doesnt present in Week controllers.
     Default value: 0;
  2. <i>groupName</i> — Name of group to get schedule.
     Default value: 19П-3.

## Examples
  1. <b>https://uksivt.azurewebsites.net/api/schedule/day?dayIndex=0&groupName=19П-3</b> — Return monday schedule for group '19П-3';
  2. <b>https://uksivt.azurewebsites.net/api/schedule/week?groupName=20ЗИО-2</b> — Return week schedule for group '20ЗИО-2';
  3. <b>https://uksivt.azurewebsites.net/api/changes/day?dayIndex=0&groupName=19П-3</b> — Return monday changes for group '19П-3';
  4. <b>https://uksivt.azurewebsites.net/api/structure/branches</b> — Return student branches;
  5. <b>https://uksivt.azurewebsites.net/api/structure/affiliates?branch=General</b> — Return student directions for branch 'General';
  6. <b>https://uksivt.azurewebsites.net/api/structure/groups?branch=Economy&affiliate=ЗИО</b> — Return student groups for branch 'Economy' and direction 'ЗИО';
  7. <b>https://uksivt.azurewebsites.net/api/structure/summary</b> — Just returns a list with all groups.
