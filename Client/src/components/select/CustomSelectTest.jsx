import React, { useEffect, useState } from 'react';
import Select from 'react-select';

export default function CustomSelectTest() {
  const [selectedOption, setSelectedOption] = useState(null);

  const handleChange = (selected) => {
    setSelectedOption(selected);
    onChange(selected);
  };

  const options = [
    { id: 99999, name: 'Lockers' },
    { id: 1, name: 'Option 1' },
    { id: 2, name: 'Option 2' },
    { id: 3, name: 'Option 3' },
  ];

  return (
    <Select
      options={options.map((option) => ({
        value: option.id,
        label: option.name
      }))}
      value={selectedOption}
      onChange={handleChange}
    />
  );
}
