import React from 'react';
import Edit from '../../popupAction/module/Edit';
import Delete from '../../popupAction/module/Delete';

function ModulePopup({ module, onClose, column }) {
  console.log(module, column, 'module in popup');
  
  return (
    <div
      className="bg-white shadow-xl rounded-lg p-5 w-72 border border-gray-200"
      onClick={(e) => e.stopPropagation()}
    >
      {module && (
        <>
          <div className="border-b border-gray-300 pb-2 mb-4">
            <h3 className="text-lg font-bold text-gray-800" style={{ textTransform: "capitalize" }}>
              {column}
            </h3>
          </div>

          {column && (
            <div className="flex flex-col gap-2">
              {column === "locker" ? (
                <Edit module={module} onClose={onClose} />
              ) : column === "module" ? (
                <Delete module={module} onClose={onClose} />
              ) : null}
            </div>
          )}
        </>
      )}
    </div>
  );
}

export default ModulePopup;
