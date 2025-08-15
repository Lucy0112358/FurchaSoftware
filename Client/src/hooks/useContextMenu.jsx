import { useState, useCallback } from 'react';
import { useSelector } from 'react-redux';
import { getManage } from '../redux/slice/systemSlice';

export function useContextMenu(selectedIds = []) {
  const manage = useSelector(getManage);
  const [popup, setPopup] = useState({
    visible: false,
    x: 0,
    y: 0,
    target: {}
  });

  const closePopup = useCallback(() => {
    setPopup(prev => ({ ...prev, visible: false }));
  }, []);

  const handleRightClick = useCallback((e, target = null) => {
    // if (!manage) return;
    e.preventDefault();
    const popupX = e.clientX + window.scrollX;
    const popupY = e.clientY + window.scrollY;

    if (!target && selectedIds.length === 0) {
      return closePopup();
    }

    setPopup({
      visible: true,
      x: popupX,
      y: popupY,
      target: target,
    });
  }, [selectedIds, closePopup, manage]);

  const handleGlobalClick = useCallback(() => {
    if (popup.visible) {
      closePopup();
    }
  }, [popup.visible, closePopup]);

  return {
    popup,
    handleRightClick,
    handleGlobalClick,
    closePopup,
  };
}
