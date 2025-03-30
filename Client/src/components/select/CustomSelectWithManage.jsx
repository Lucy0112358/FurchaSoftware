import React, { useState } from 'react';
import Select from 'react-select';

export default function CustomSelectWithManage({
  options,
  onChange = () => {},
  multiChoose = false,
  selectedOption,
  setSelectedOption,
}) {
  const isControlled = selectedOption !== undefined && typeof setSelectedOption === 'function';
  const [localSelectedOption, setLocalSelectedOption] = useState(null);

  const handleChange = (selected) => {
    if (isControlled) {
      setSelectedOption(selected);
    } else {
      setLocalSelectedOption(selected);
    }
    onChange(selected);
  };

  return (
    <Select
      options={Array.isArray(options) ? options.map((option) => ({
        value: option.name === "All" ? "" : option.id,
        label: option.name,
      })) : []}
      isMulti={multiChoose}
      value={isControlled ? selectedOption : localSelectedOption}
      onChange={handleChange}
    />
  );
}
