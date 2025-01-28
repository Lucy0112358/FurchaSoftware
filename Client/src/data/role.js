const initialRights = {
    user: [
      { id: 'manageUsers', label: 'Manage Users', checked: false },
      { id: 'manageUGroups', label: 'Manage U. Groups', checked: false },
      { id: 'exportImport', label: 'Export/Import', checked: false },
    ],
    adminLevl: [
      { id: 'lvl5', label: 'LVL5 Master Admin', checked: false },
      { id: 'lvl4', label: 'LVL4 Super Admin', checked: false },
      { id: 'lvl3', label: 'LVL3', checked: false },
    ],
    locker: [
      { id: 'manageLG', label: 'Manage L. Groups', checked: false },
      { id: 'manageType', label: 'Manage Type', checked: false },
      { id: 'assignTemp', label: 'Assign Temp Pers', checked: false },
      { id: 'storeParcel', label: 'Store Parcel', checked: false },
    ],
    openLocker : {
      main: [
        { id: 'personal', label: 'Personal', open: true, free: false },
        { id: 'common', label: 'Common', open: false, free: true },
        { id: 'swap', label: 'Swap', open: false, free: false },
        { id: 'parcel', label: 'Parcel', open: true, free: false },
        { id: 'unspec', label: 'Unspec', open: true, free: true },
      ],
    },
    moduleChains: [
      { id: 'manageChains', label: 'Manage Chains', checked: false },
    ],
  };