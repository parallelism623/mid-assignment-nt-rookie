import React, { useState, useEffect } from "react";
import { Modal, DatePicker, Button } from "antd";
import dayjs from "dayjs";

export default function ExtendDueDateModal({
  visible,
  currentDueDate,
  onExtend,
  onCancel,
  bookBorrowedId,
}) {
  const [newDate, setNewDate] = useState(dayjs());
  useEffect(() => {
    if (visible) {
      let defaultDate = currentDueDate
        ? dayjs(currentDueDate).add(1, "day")
        : dayjs().add(1, "day");
      if (defaultDate.isBefore(dayjs())) {
        defaultDate = dayjs().add(1, "day");
      }
      setNewDate(defaultDate);
    }
  }, [visible, currentDueDate]);

  const handleOk = () => {
    if (newDate) {
      onExtend(newDate.format("YYYY-MM-DD"), bookBorrowedId);
      onCancel();
    }
  };
  const minDate = dayjs().add(1, "day").startOf("day");

  return (
    <Modal
      title="Extend Due Date"
      visible={visible}
      onCancel={onCancel}
      footer={[
        <Button key="cancel" onClick={onCancel}>
          Cancel
        </Button>,
        <Button key="extend" type="primary" onClick={handleOk}>
          Extend
        </Button>,
      ]}
    >
      <p>Select new due date:</p>
      <DatePicker
        value={newDate}
        onChange={(date) => setNewDate(date)}
        style={{ width: "100%" }}
        disabledDate={(current) => current && current.isBefore(minDate, "day")}
      />
    </Modal>
  );
}
