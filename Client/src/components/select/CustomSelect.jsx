import React from 'react';
import Select from 'react-select';

export default function CustomSelect({ options, onChange, value, multiChoose = false, defaultValue= {} }) {
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
        value: option.label === "All" ? "" : option.value,
        label: option.label
      })) : []}
      defaultValue={defaultValue}
      isMulti={multiChoose}
      value={value}
      onChange={onChange}
    />
  );
}
