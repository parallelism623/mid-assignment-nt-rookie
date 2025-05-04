import { Tag, Card, Table, Tooltip, Row, Button, Col, Popconfirm } from "antd";
import { useEffect, useState } from "react";
import { defaultQueryParameters } from "../../constants/queryParameters";
import { categoryServices } from "../../services/categoryServices";
import { FiEdit, FiTrash2, FiEye } from "react-icons/fi";
import CategoryEdit from "./CategoryEdit";
import CreateCategory from "./CreataCategory";
import CategoryDetail from "./CategoryDetail";
import ConfirmModal from "../../components/ui/ConfirmModal";
const { Column } = Table;

const Category = () => {
  const [categories, setCategories] = useState([]);
  const [modalVisible, setModalVisible] = useState(false);
  const [saving, setSaving] = useState(false);
  const [loading, setLoading] = useState(false);
  const [categoryEditingId, setCategoryEditingId] = useState(null);
  const [openDeleteConfirmModal, setOpenDeleteConfirmModal] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [categoryViewDetailId, setCategoryViewDetailId] = useState(null);
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameters,
  });
  const [openCreateCategoryForm, setOpenCreateCategoryForm] = useState(false);

  const fetchData = (isSetData = true) => {
    setLoading(true);
    categoryServices
      .gets(queryParameters)
      .then((res) => {
        if (isSetData) {
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setCategories(res.items);
        }
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
      });
  };
  useEffect(() => {
    let isSetData = true;
    fetchData(isSetData);
    return () => {
      isSetData = false;
    };
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.totalCount,
    queryParameters.search,
  ]);
  const handleOnEdit = (id) => {
    setCategoryEditingId(id);
    setModalVisible(true);
  };

  const handlePageChange = (pageIndex, pageSize) => {
    setQueryParameters({
      ...queryParameters,
      pageIndex: pageIndex,
      pageSize: pageSize,
    });
  };
  const handleOnSaveCategory = (updateModel) => {
    setSaving(true);
    categoryServices
      .update(updateModel)
      .then(() => {
        categoryServices.getsName().then((res) => {
          setQueryParameters({
            ...queryParameters,
            totalCount: res.totalCount,
          });
          setCategories(res.items);
        });
        setSaving(false);
      })
      .catch(() => {
        setSaving(true);
      });
  };
  const onSubmitCreateForm = () => {
    setOpenCreateCategoryForm(false);
    fetchData();
  };
  const onCancelCreateForm = () => {
    setOpenCreateCategoryForm(false);
  };
  const handleOnClickDeleteButton = (record) => {
    setSelectedCategory(record);
    setOpenDeleteConfirmModal(true);
  };
  const handleOnCancelDeleteForm = () => {
    setSelectedCategory(null);
    setOpenDeleteConfirmModal(false);
  };
  const handleOnSubmitDelete = () => {
    setOpenDeleteConfirmModal(false);
    categoryServices.delete(selectedCategory.id).then((res) => {
      setQueryParameters({
        ...queryParameters,
        totalCount: queryParameters.totalCount - 1,
      });
    });
  };
  const handleViewDetail = (id) => {
    setCategoryViewDetailId(id);
  };
  return (
    <>
      {openDeleteConfirmModal && (
        <ConfirmModal
          visitable={openDeleteConfirmModal}
          title={"Confirm Category Deletion"}
          message={`Are you sure you want to delete the category ${selectedCategory.name}? This action cannot be undone`}
          onCancel={handleOnCancelDeleteForm}
          onSubmit={handleOnSubmitDelete}
        />
      )}
      <div className="flex gap-4 p-4">
        <div className="flex-1 overflow">
          {modalVisible && (
            <CategoryEdit
              visible={modalVisible}
              confirmLoading={saving}
              onCancel={() => {
                setModalVisible(false);
                setCategoryEditingId(null);
              }}
              categoryId={categoryEditingId}
              onSave={handleOnSaveCategory}
            />
          )}
          {openCreateCategoryForm && (
            <CreateCategory
              onCancel={onCancelCreateForm}
              onSubmit={onSubmitCreateForm}
            />
          )}
          {!!categoryViewDetailId && (
            <CategoryDetail
              categoryId={categoryViewDetailId}
              onCancel={() => {
                setCategoryViewDetailId(null);
              }}
            />
          )}
          <Card
            extra={
              <Row className="flex items-center justify-between gap-1.5 overflow-x-hidden">
                <Col>
                  <Button
                    onClick={() => setOpenCreateCategoryForm(true)}
                    className="create-book-button"
                  >
                    Create Category
                  </Button>
                </Col>
              </Row>
            }
            title="Categories"
            className="book-list-card be-vietnam-pro-regular"
          >
            <Table
              dataSource={categories}
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
              <Column
                onHeaderCell={() => ({
                  className: "be-vietnam-pro-medium",
                })}
                title="Name"
                dataIndex="name"
                key="name"
                ellipsis
              />
              <Column
                title="Description"
                dataIndex="description"
                key="description"
                ellipsis
              />
              <Column
                onHeaderCell={() => ({
                  className: "be-vietnam-pro-medium",
                })}
                title="Quantity"
                dataIndex="quantityBooks"
                key="quantityBooks"
                render={(value) => {
                  return <Tag color="yellow">{value}</Tag>;
                }}
                align="center"
              />
              <Column
                onHeaderCell={() => ({
                  className: "be-vietnam-pro-medium",
                })}
                title="Available"
                dataIndex="availableBooks"
                key="availableBooks"
                render={(value) => {
                  return <Tag color="green">{value}</Tag>;
                }}
                align="center"
              />
              <Column
                onHeaderCell={() => ({
                  className: "be-vietnam-pro-medium",
                })}
                title="Action"
                key="action"
                render={(_, record) => (
                  <div className="flex gap-2">
                    <Tooltip title="Edit">
                      <button
                        onClick={() => handleOnEdit(record.id)}
                        className="p-2 rounded-lg bg-blue-200 text-blue-800 hover:bg-blue-300 transition"
                      >
                        <FiEdit size={18} />
                      </button>
                    </Tooltip>

                    <Tooltip title="Delete">
                      <button
                        onClick={() => {
                          handleOnClickDeleteButton(record);
                        }}
                        className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400 transition"
                      >
                        <FiTrash2 size={18} />
                      </button>
                    </Tooltip>

                    <Tooltip title="Detail">
                      <button
                        onClick={() => handleViewDetail(record.id)}
                        className="p-2 rounded-lg bg-emerald-300 text-emerald-800 hover:bg-emerald-400 transition"
                      >
                        <FiEye size={18} />
                      </button>
                    </Tooltip>
                  </div>
                )}
              />
            </Table>
          </Card>
        </div>
      </div>
    </>
  );
};

export default Category;
