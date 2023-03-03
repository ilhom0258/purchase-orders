import React, { Component } from 'react';
import PurchaseOrder from './PurchaseOrder';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
        <PurchaseOrder/>
    );
  }
}
