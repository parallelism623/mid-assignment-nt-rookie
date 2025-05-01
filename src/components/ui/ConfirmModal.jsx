import { Modal } from "antd";
const ConfirmModal = ({
  title,
  message,
  onSubmit,
  onCancel,
  visitable,
  submitText = "Save",
  cancelText = "Cancel",
}) => {
  return (
    <>
      <Modal
        title={title}
        open={visitable}
        onOk={onSubmit}
        onCancel={onCancel}
        okText={submitText}
        cancelText={cancelText}
      >
        {message}
      </Modal>
    </>
  );
};

export default ConfirmModal;
