import { Card, Table, Row, Col, Button, Input } from "antd";

import { useEffect, useState } from "react";
import { userServices } from "../../services/userServices";
import ViewDetailButton from "../../components/ui/buttons/ViewDetailButton";
import DeleteButton from "../../components/ui/buttons/DeleteButton";
import EditButton from "../../components/ui/buttons/EditButton";
import ConfirmModal from "../../components/ui/ConfirmModal";
import EditUserModal from "./EditUser";
import CreateUserModal from "./CreateUser";
import RoleSelect from "./RoleSelect";
const { Search } = Input;
const defaultQueryParameters = {
  pageIndex: 1,
  pageSize: 10,
  search: "",
  roleIds: "",
};
const { Column } = Table;
const User = () => {
  const [queryParameters, setQueryParameters] = useState(
    defaultQueryParameters
  );
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [openDeleteConfirmModal, setOpenDeleteConfirmModal] = useState(false);
  const [openEditModal, setOpenEditModal] = useState(false);
  const [openCreateModal, setOpenCreateModal] = useState(false);
  const [userSelected, setUserSelected] = useState(null);
  const fetchData = (isSetData = true) => {
    userServices.get(queryParameters).then((res) => {
      if (isSetData) {
        setUsers(res.items);
        setQueryParameters({ ...queryParameters, totalCount: res.totalCount });
      }
      setLoading(false);
    });
  };

  useEffect(() => {
    let isSetData = true;
    fetchData();

    return () => (isSetData = false);
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.search,
    queryParameters.roleIds,
  ]);
  const handlePageChange = (pageIndex, pageSize) => {
    setQueryParameters({ ...queryParameters, pageIndex, pageSize });
  };
  const handleOnClickDeleteButton = (record) => {
    setUserSelected(record);
    setOpenDeleteConfirmModal(true);
  };
  const handleOnClickEditButton = (record) => {
    setOpenEditModal(true);
    setUserSelected(record);
  };
  const handleOnClickCreateButton = () => {
    setOpenCreateModal(true);
  };
  const handleOnEnterSearch = (value) => {
    setQueryParameters({ ...queryParameters, search: value });
  };
  const handleOnClickViewDetailButton = () => {};
  const handleOnSubmitDeleteUser = async () => {
    setLoading(true);
    await userServices.delete(userSelected?.id);
    setLoading(false);
    setOpenDeleteConfirmModal(false);
    fetchData();
  };
  const handleOnSubmitCreateUser = async (user) => {
    setLoading(true);
    userServices
      .create(user)
      .then(() => {
        setLoading(false);
        setOpenCreateModal(false);
        fetchData();
      })
      .catch(() => {
        setLoading(false);
        setOpenCreateModal(false);
      });
  };
  const handleOnCancelCreateUser = () => {
    setOpenCreateModal(false);
  };
  const handleOnSubmitUpdateUser = async (user) => {
    setLoading(true);
    await userServices.update({ ...userSelected, ...user });
    setLoading(false);
    setOpenEditModal(false);
    fetchData();
  };
  const handleOnCancelUpdateUser = () => {
    setOpenEditModal(false);
  };
  const handleOnDeleteUser = async () => {
    setOpenDeleteConfirmModal(false);
  };
  const handleOnSelectRole = (values) => {
    console.log(values.join(";"));
    setQueryParameters({
      ...Button.queryParameters,
      roleIds: values.join(";"),
    });
  };
  return (
    <>
      {openDeleteConfirmModal && (
        <ConfirmModal
          visitable={openDeleteConfirmModal}
          title={"Confirm User Deletion"}
          message={`Are you sure you want to delete the user ${userSelected?.username}? This action cannot be undone`}
          onSubmit={handleOnSubmitDeleteUser}
          onCancel={handleOnDeleteUser}
        />
      )}
      {openEditModal && (
        <EditUserModal
          visible={openEditModal}
          initialValues={userSelected}
          onCancel={handleOnCancelUpdateUser}
          onSubmit={handleOnSubmitUpdateUser}
        ></EditUserModal>
      )}

      {openCreateModal && (
        <CreateUserModal
          visible={openCreateModal}
          onSubmit={handleOnSubmitCreateUser}
          onCancel={handleOnCancelCreateUser}
        ></CreateUserModal>
      )}
      <Card
        extra={
          <Row className="flex items-center justify-between gap-1.5 overflow-x-hidden">
            <Col>
              <RoleSelect onSelect={handleOnSelectRole} />
            </Col>
            <Col>
              <Search
                placeholder="Search by name/email"
                style={{ width: 240 }}
                onSearch={handleOnEnterSearch}
                defaultValue={queryParameters.search}
                loading={loading}
              />
            </Col>
            <Col>
              <Button
                className="create-book-button"
                onClick={handleOnClickCreateButton}
              >
                Create User
              </Button>
            </Col>
          </Row>
        }
        title="Users"
        className="book-list-card be-vietnam-pro-regular"
      >
        <Table
          dataSource={users}
          rowKey="id"
          loading={loading}
          pagination={{
            current: queryParameters.pageIndex,
            total: queryParameters.totalCount,
            pageSize: queryParameters.pageSize,
            showSizeChanger: true,
            pageSizeOptions: [5, 10, 20, 50],
            showTotal: (total, [start, end]) =>
              `From ${start} to ${end} items of ${total}`,
            onChange: (pageIndex, pageSize) => {
              handlePageChange(pageIndex, pageSize);
            },
          }}
          bordered
        >
          <Column title="Username" dataIndex="username" key="username" />
          <Column title="Email" dataIndex="email" key="email" />
          <Column title="Role" dataIndex="roleName" key="roleName" />
          <Column title="First Name" dataIndex="firstName" key="firstName" />
          <Column title="Last Name" dataIndex="lastName" key="lastName" />
          <Column
            title="Phone Number"
            dataIndex="phoneNumber"
            key="phoneNumber"
          />
          <Column
            title="Book Borrow Limit"
            dataIndex="bookBorrowingLimit"
            key="bookBorrowingLimit"
          />
          <Column
            key="action"
            title="Action"
            onHeaderCell={() => ({
              className: "be-vietnam-pro-medium",
            })}
            render={(_, record) => (
              <div className="flex gap-2">
                <EditButton
                  onClick={() => handleOnClickEditButton(record)}
                ></EditButton>
                <DeleteButton
                  onClick={() => handleOnClickDeleteButton(record)}
                ></DeleteButton>
                <ViewDetailButton
                  onClick={handleOnClickViewDetailButton}
                ></ViewDetailButton>
              </div>
            )}
          />
        </Table>
      </Card>
    </>
  );
};

export default User;
