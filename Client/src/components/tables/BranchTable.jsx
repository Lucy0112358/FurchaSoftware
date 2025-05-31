import React, { useEffect, useRef, useState } from 'react';
import { branchTable } from '../../data/tableIHeads';
import { useSelector, useDispatch } from 'react-redux';
import NoData from '../no-data/NoData';
import { getAllBranchesData } from '../../redux/slice/branchSlice';
import BranchPopup from '../popups/branch/BranchPopup';

function BranchTable() {
  const allBranches = useSelector(getAllBranchesData);
  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    branch: null,
  });

  const handleRightClick = (e, branch = null) => {
    e.preventDefault();
    const popupX = e.clientX + window.scrollX;
    const popupY = e.clientY + window.scrollY;

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      branch,
    });
  };

  const popupRef = useRef(null);

  const closePopup = () => {
    setPopup((prev) => ({ ...prev, visible: false }));
  };

  const handleGlobalClick = () => {
    if (popup.visible) closePopup();
  };

  useEffect(() => {
    const handleClickOutside = (e) => {
      if (popup.visible && popupRef.current && !popupRef.current.contains(e.target)) {
        closePopup();
      }
    };

    window.addEventListener('click', handleClickOutside);
    return () => {
      window.removeEventListener('click', handleClickOutside);
    };
  }, [popup.visible]);

  return (
    <div
      className="select-none"
      style={{ marginRight: '4.9rem' }}
      onClick={handleGlobalClick}
      onContextMenu={(e) => e.preventDefault()}
    >
      {allBranches?.length ? (
        <div
          className="outlet__table__wrapper overflow-x-auto"
          style={{ height: allBranches.length >= 10 ? '480px' : 'auto' }}
        >
          <table
            className="outlet__table min-w-full bg-white"
            style={{ color: '#AAAAAA' }}
          >
            <thead>
              <tr className="outlet__table__header">
                {branchTable.map((header, index) => (
                  <th key={index} className="text-left">
                    {header}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {allBranches.map((branch, index) => (
                <tr
                  key={branch.id}
                  onContextMenu={(e) => handleRightClick(e, branch)}
                  className={index % 2 === 0 ? 'bg-gray-50' : 'bg-white'}
                >
                  <td>
                    {branch.id}
                  </td>
                  <td>{branch.name}</td>
                  <td>{branch.address}</td>
                  <td>{branch.comment}</td>
                  <td>{branch.lockerTypes?.toString()}</td>
                  <td>{branch.lockersCount}</td>
                  <td className={branch.mode === 0 ? 'text-red-500' : 'text-green-500'}>
                    {branch.mode === 0 ? 'Inactive' : 'Active'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <NoData text="No Branches" />
      )}
      {popup.visible && (
        <div
          ref={popupRef}
          style={{
            position: 'absolute',
            top: popup.y,
            left: popup.x,
            zIndex: 999,
          }}
          onClick={(e) => e.stopPropagation()}
        >
          <BranchPopup branch={popup.branch} onClose={closePopup} />
        </div>
      )}
    </div>
  );
}

export default BranchTable;