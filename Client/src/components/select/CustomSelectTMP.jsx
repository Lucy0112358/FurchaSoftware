import React, { useState } from 'react';
import Select from 'react-select';

export default function CustomSelect({ options, onChange, multiChoose = false }) {
  const [selectedOption, setSelectedOption] = useState(null);

  const handleChange = (selected) => {
    setSelectedOption(selected);
    onChange(selected);
  };

  return (
    <Select
      styles={{
        option: (provided) => ({
          ...provided,
          whiteSpace: 'normal',
          wordBreak: 'break-word',
        }),
        singleValue: (provided) => ({
          ...provided,
          whiteSpace: 'normal',
          wordBreak: 'break-word',
        }),
      }}
      options={Array.isArray(options) ? options.map((option) => ({
        value: option.name == "All" ? "" : option.id,
        label: option.name
      })) : []}
      isMulti={multiChoose}
      value={selectedOption}
      onChange={handleChange}
    />
  );
}