import React, { useState, useEffect } from "react";
import useToken from "../hooks/userHooks";
import {
  Table,
  Button,
  Alert,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  Input,
  InputGroup,
  InputGroupText,
  ListGroup,
  ListGroupItem,
  ButtonGroup,
} from "reactstrap";

function PurchaseOrder() {
  const { token } = useToken();
  const [purchaseOrders, setPurchaseOrders] = useState([]);
  const [selectedPurchaseOrder, setSelectedPurchaseOrder] = useState(null);
  const [purchaseOrderName, setPurchaseOrderName] = useState("");
  const [purchaseOrderLineItems, setPurchaseOrderLineItems] = useState([]);
  const [lineItemName, setLineItemName] = useState("");
  const [lineItemAmount, setLineItemAmount] = useState(0);
  const [errorMessage, setErrorMessage] = useState(null);
  const [purchaseOrderModal, setPurchaseOrderModal] = useState(false);

  // Fetch all purchase orders on component mount
  useEffect(() => {
    fetch("api/purchaseorder", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          response.json().then((v) => {
            setError(v.error);
          });
          return null;
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setPurchaseOrders(data);
        }
      });
  }, [token]);

  // Handler for creating a new purchase order
  const handleCreatePurchaseOrder = () => {
    const newPurchaseOrder = {
      name: purchaseOrderName,
      lineItems: purchaseOrderLineItems,
    };

    fetch("api/purchaseorder", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(newPurchaseOrder),
    })
      .then((response) => {
        if (!response.ok) {
          response.json().then((v) => {
            setError(v.error);
          });
          return null;
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setPurchaseOrders([...purchaseOrders, data]);
          setPurchaseOrderName("");
          setPurchaseOrderLineItems([]);
        }
      });
  };

  // Handler for editing a selected purchase order's line items
  const handleEditPurchaseOrderLineItems = () => {
    const editedPurchaseOrder = {
      ...selectedPurchaseOrder,
      lineItems: purchaseOrderLineItems,
    };

    fetch(`api/purchaseorder/${selectedPurchaseOrder.id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(editedPurchaseOrder),
    })
      .then((response) => {
        if (!response.ok) {
          response.json().then((v) => {
            setError(v.error);
          });
          return null;
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setPurchaseOrders(
            purchaseOrders.map((po) => (po.id === data.id ? data : po))
          );
          setSelectedPurchaseOrder(null);
          setPurchaseOrderLineItems([]);
        }
      });
  };

  // Handler for submitting a selected purchase order
  const handleSubmitPurchaseOrder = (selectedPurchaseOrder) => {
    fetch(`api/purchaseorder/${selectedPurchaseOrder.id}/submit`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          response.json().then((v) => {
            setError(v.error);
          });
          return null;
        }
        return response.json();
      })
      .then((data) => {
        if (data) {
          setPurchaseOrders(
            purchaseOrders.map((po) => (po.id === data.id ? data : po))
          );
          setSelectedPurchaseOrder(null);
        }
      });
  };

  // Handler for deleting a selected purchase order
  const handleDeletePurchaseOrder = (purchaseOrder) => {
    fetch(`api/purchaseorder/${purchaseOrder.id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }).then((response) => {
      if (!response.ok) {
        response.json().then((v) => {
          setError(v.error);
        });
        return null;
      } else {
        setPurchaseOrders(
          purchaseOrders.filter((po) => po.id !== purchaseOrder.id)
        );
        setSelectedPurchaseOrder(null);
      }
    });
  };

  // Handler for selecting a purchase order to edit
  const handleSelectPurchaseOrder = (purchaseOrder) => {
    setSelectedPurchaseOrder(purchaseOrder);
    setPurchaseOrderName(purchaseOrder.name);
    setPurchaseOrderLineItems(purchaseOrder.lineItems);
    setPurchaseOrderModal(!purchaseOrderModal);
  };

  // Handler for adding a new line item to the purchase order
  const handleAddLineItem = () => {
    const newLineItem = {
      name: lineItemName,
      amount: lineItemAmount,
    };

    setPurchaseOrderLineItems([...purchaseOrderLineItems, newLineItem]);
    setLineItemName("");
    setLineItemAmount(0);
  };

  const setError = (error) => {
    setErrorMessage(error);
    return setTimeout(() => {
      setErrorMessage(null);
    }, 3000);
  };

  const alertVisible = errorMessage ? true : false;
  const onAlertDismiss = () => setErrorMessage(null);

  const showModal = () => setPurchaseOrderModal(!purchaseOrderModal);

  return (
    <div>
      <div>
        {errorMessage ? (
          <Alert color="danger" isOpen={alertVisible} toggle={onAlertDismiss}>
            {errorMessage}
          </Alert>
        ) : null}
      </div>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
        }}
      >
        <h1>Purchase Orders</h1>
        <Button onClick={handleCreatePurchaseOrder}>Create</Button>
      </div>
      {/* List of purchase orders */}
      <Table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Line Items</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {purchaseOrders.map((purchaseOrder) => (
            <tr key={purchaseOrder.id}>
              <td>{purchaseOrder.name}</td>
              <td>
                  <ListGroup>
                    {purchaseOrder.lineItems.map((lineItem, index) => (
                            <div key={index}>
                                <ListGroupItem>{lineItem.name}:{lineItem.amount}</ListGroupItem>
                            </div>
                    ))}
                  </ListGroup>
              </td>
              <td>
                <ButtonGroup
                  style={{
                    display: "flex",
                    gap: "10px",
                  }}
                >
                  <Button
                    color="info"
                    onClick={() => handleSelectPurchaseOrder(purchaseOrder)}
                  >
                    Edit
                  </Button>
                  <Button
                    color="danger"
                    onClick={() => handleDeletePurchaseOrder(purchaseOrder)}
                  >
                    Delete
                  </Button>
                  {purchaseOrder.status !== "SUBMITTED" && (
                    <Button
                      color="primary"
                      onClick={() => handleSubmitPurchaseOrder(purchaseOrder)}
                    >
                      Submit
                    </Button>
                  )}
                </ButtonGroup>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <PurchaseOrderModal
        selectedPurchaseOrder={selectedPurchaseOrder}
        purchaseOrderName={purchaseOrderName}
        purchaseOrderLineItems={purchaseOrderLineItems}
        handleAddLineItem={handleAddLineItem}
        setPurchaseOrderName={setPurchaseOrderName}
        setPurchaseOrderLineItems={setPurchaseOrderLineItems}
        setLineItemName={setLineItemName}
        setLineItemAmount={setLineItemAmount}
        lineItemAmount={lineItemAmount}
        lineItemName={lineItemName}
        setSelectedPurchaseOrder={setSelectedPurchaseOrder}
        handleEditPurchaseOrderLineItems={handleEditPurchaseOrderLineItems}
        handleCreatePurchaseOrder={handleCreatePurchaseOrder}
        purchaseOrderModal={purchaseOrderModal}
        showModal={showModal}
      />
      {/* Form for creating/editing purchase order */}
    </div>
  );
}

function PurchaseOrderModal({
  selectedPurchaseOrder,
  purchaseOrderName,
  purchaseOrderLineItems,
  handleAddLineItem,
  setPurchaseOrderName,
  setPurchaseOrderLineItems,
  setSelectedPurchaseOrder,
  handleEditPurchaseOrderLineItems,
  handleCreatePurchaseOrder,
  purchaseOrderModal,
  showModal,
}) {
  return (
    selectedPurchaseOrder &&
    purchaseOrderModal && (
      <Modal isOpen={purchaseOrderModal} toggle={showModal}>
        <ModalHeader toggle={showModal}>
          {selectedPurchaseOrder.id ? "Edit" : "Create"} Purchase Order
        </ModalHeader>
        <ModalBody>
          {selectedPurchaseOrder && (
            <div
              style={{
                display: "flex",
                flexDirection: "column",
                gap: "10px",
              }}
            >
              <InputGroup>
                <InputGroupText>PO Name:</InputGroupText>
                <Input
                  type="text"
                  value={purchaseOrderName}
                  onChange={(e) => setPurchaseOrderName(e.target.value)}
                />
              </InputGroup>

              <InputGroup>
                <div
                  style={{
                    display: "flex",
                    flexDirection: "row",
                  }}
                >
                  <div
                    style={{
                      flex: 0.2,
                    }}
                  >
                    Line Items:
                  </div>
                  <div
                    style={{
                      flex: 0.8,
                      display: "flex",
                      flexDirection: "column",
                      gap: "10px",
                    }}
                  >
                    <ListGroup>
                      {purchaseOrderLineItems.map((lineItem, index) => (
                        <ListGroupItem key={index}>
                          <div
                            style={{
                              display: "flex",
                              flexDirection: "row",
                              gap: "5px",
                            }}
                          >
                            <Input
                              size={"sm"}
                              type="text"
                              value={lineItem.name}
                              onChange={(e) => {
                                const newLineItems = [
                                  ...purchaseOrderLineItems,
                                ];
                                newLineItems[index] = {
                                  ...newLineItems[index],
                                  name: e.target.value,
                                };
                                setPurchaseOrderLineItems(newLineItems);
                              }}
                            />
                            <Input
                              size={"sm"}
                              type="number"
                              value={lineItem.amount}
                              onChange={(e) => {
                                const newLineItems = [
                                  ...purchaseOrderLineItems,
                                ];
                                newLineItems[index] = {
                                  ...newLineItems[index],
                                  amount: e.target.value,
                                };
                                setPurchaseOrderLineItems(newLineItems);
                              }}
                            />
                            <Button
                              size="sm"
                              color="danger"
                              onClick={() => {
                                const newLineItems = [
                                  ...purchaseOrderLineItems,
                                ];
                                newLineItems.splice(index, 1);
                                setPurchaseOrderLineItems(newLineItems);
                              }}
                            >
                              Remove
                            </Button>
                          </div>
                        </ListGroupItem>
                      ))}
                      <InputGroup>
                            <Button block onClick={handleAddLineItem}>+</Button>
                            <InputGroup>
                                <InputGroupText>Total amount</InputGroupText>
                                <Input style={{textAlign:'center'}} disabled type="number" value={purchaseOrderLineItems.reduce((ac, li) => {
                                    return Number(ac) + Number(li.amount);
                                }, 0)}></Input>
                            </InputGroup>
                      </InputGroup>
                    </ListGroup>
                  </div>
                </div>
              </InputGroup>
            </div>
          )}
        </ModalBody>
        <ModalFooter>
          <Button
            color="primary"
            onClick={
              selectedPurchaseOrder.id
                ? handleEditPurchaseOrderLineItems
                : handleCreatePurchaseOrder
            }
          >
            {selectedPurchaseOrder.id
              ? "Save Changes"
              : "Create Purchase Order"}
          </Button>{" "}
          <Button
            color="secondary"
            onClick={() => setSelectedPurchaseOrder(null)}
          >
            Cancel
          </Button>
        </ModalFooter>
      </Modal>
    )
  );
}

export default PurchaseOrder;
