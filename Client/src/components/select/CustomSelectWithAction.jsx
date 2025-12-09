import React from "react";
import Select, { components } from "react-select";

export default function CustomSelectWithAction({
  options,
  onChange,
  value,
  multiChoose = false,
  defaultValue = {},
  onDeleteOption,
}) {

  const Option = (props) => {
    const { data } = props;

    const handleDelete = (e) => {
      e.stopPropagation();
      if (onDeleteOption) {
        onDeleteOption(data);
      }
    };

    return (
      <components.Option {...props}>
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <span>{data.label}</span>
          <button
            onClick={handleDelete}
            style={{
              background: "transparent",
              border: "none",
              cursor: "pointer",
              marginLeft: 10,
              fontSize: 14
            }}
          >
            ❌
          </button>
        </div>
      </components.Option>
    );
  };

  return (
    <Select
      components={{ Option }}
      styles={{
        option: (provided) => ({
          ...provided,
          whiteSpace: "normal",
          wordBreak: "break-word",
        }),
      }}
      options={options}
      defaultValue={defaultValue}
      isMulti={multiChoose}
      value={value}
      onChange={onChange}
    />
  );
}
