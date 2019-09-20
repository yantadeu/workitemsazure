import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { actionCreators } from '../store/WeatherForecasts';
import Toolbar from '@material-ui/core/Toolbar';
import Input from '@material-ui/core/Input';
import { orderBy } from 'lodash';
import Grid from '@material-ui/core/Grid';

class FetchData extends Component {
  state = { filter: undefined, order: 'desc' };
  componentWillMount() {
    // This method runs when the component is first added to the page
    const startDateIndex = parseInt(this.props.match.params.startDateIndex, 10) || 0;
    this.props.requestWeatherForecasts(startDateIndex);
  }

  componentWillReceiveProps(nextProps) {
    // This method runs when incoming props (e.g., route params) change
    const startDateIndex = parseInt(nextProps.match.params.startDateIndex, 10) || 0;
    this.props.requestWeatherForecasts(startDateIndex);
  }
  updateInputValue = (evt) => {
    this.setState({
        filter: evt.target.value,
    });
    }
    toggleOrder = (order) => {
        this.setState({
            order: order
        });
    }
    render() {
        const { forecasts } = this.props;
        const { filter, order } = this.state;
        console.log(forecasts, order);
        const list = orderBy([...forecasts], ['created_at'], [order]).filter((h) => {
            let no = false;
            if (h.type && !filter) {
                no = false;
            }
            if (filter && h.type) {
                no = !filter.toLowerCase().startsWith(h.type.toLowerCase());
            }
            return !no;
        });
    
    return (
      <div>
        <h1>Work Items Azure devOps</h1>
        <p>Este componente exibe as informacoes do banco de dados alimentado pela API do Azure</p>
            <Toolbar className='clearfix' style={{ borderBottom: '1px solid #e0e0e0', backgroudColor: "#ccc", display: "flex", flexDirection: "row", justifyContent: "space-between", alignItems: "center" }}>
                <div style={{ display: "flex", flex: 1, flexDirection: "row", justifyContent: "space-between", alignItems: "center" }}>
                    <div style={{ display: "flex", flex: 1 }}>
                        <Input style={{ display: "flex", flex: 1, fontSize: 18 }} value={this.state.filter} placeholder="Filtrar por tipo" onChange={this.updateInputValue} />
                    </div>
                    <div style={{ display: "flex", flexDirection: "row", justifyContent: "space-between", alignItems: "center" }}>
                    <div>Ordenar por data</div>
                    <a className='btn' onClick={() => this.toggleOrder('asc')}>ASC</a>
                    <a className='btn' onClick={() => this.toggleOrder('desc')}>DESC</a>
                    </div>
                </div>
        </Toolbar>
        {renderForecastsTable(list)}
        {renderPagination(this.props)}
      </div>
    );
  }
}

function renderForecastsTable(forecasts) {
  return (
    <table className='table'>
      <thead>
        <tr>
          <th>Id</th>
          <th>Tipo</th>
          <th>Titulo</th>
          <th>Data de criacao</th>
        </tr>
      </thead>
      <tbody>
        {forecasts.map(forecast =>
          <tr key={forecast.id}>
            <td>{forecast.id}</td>
            <td>{forecast.type}</td>
            <td>{forecast.title}</td>
            <td>{forecast.created_at}</td>
          </tr>
        )}
      </tbody>
    </table>
  );
}

function renderPagination(props) {
    const prevStartDateIndex = props.startDateIndex > 5 ?(props.startDateIndex || 0) - 5: 0;
  const nextStartDateIndex = (props.startDateIndex || 0) + 5;

  return <p className='clearfix text-center'>
    <Link className='btn btn-default pull-left' to={`/fetchdata/${prevStartDateIndex}`}>Anterior</Link>
    <Link className='btn btn-default pull-right' to={`/fetchdata/${nextStartDateIndex}`}>Proximo</Link>
    {props.isLoading ? <span>Loading...</span> : []}
  </p>;
}

export default connect(
  state => state.weatherForecasts,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(FetchData);
