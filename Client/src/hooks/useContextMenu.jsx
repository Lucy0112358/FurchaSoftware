import { useState, useCallback } from 'react';

export function useContextMenu(selectedIds = []) {
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
  }, [selectedIds, closePopup]);

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
