import defaults from 'lodash/defaults';

import React, { ChangeEvent, PureComponent } from 'react';
import { LegacyForms } from '@grafana/ui';
import { QueryEditorProps } from '@grafana/data';
import { DataSource } from './DataSource';
import { defaultQuery, MyDataSourceOptions, MyQuery } from './types';

const { FormField } = LegacyForms;

type Props = QueryEditorProps<DataSource, MyQuery, MyDataSourceOptions>;

export class QueryEditor extends PureComponent<Props> {
  // onQueryTextChange = (event: ChangeEvent<HTMLInputElement>) => {
  //   const { onChange, query } = this.props;
  //   onChange({ ...query, queryText: event.target.value });
  // };

  onServerChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { onChange, query, onRunQuery } = this.props;
    onChange({ ...query, server: event.target.value });
    // executes the query
    onRunQuery();
  };
  onCapacityChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { onChange, query, onRunQuery } = this.props;
    onChange({ ...query, capacity: parseInt(event.target.value, 10) });
    // executes the query
    onRunQuery();
  };
  onTimeoutChange = (event: ChangeEvent<HTMLInputElement>) => {
    const { onChange, query, onRunQuery } = this.props;
    onChange({ ...query, timeoutS: parseInt(event.target.value, 10) });
    // executes the query
    onRunQuery();
  };

  render() {
    const query = defaults(this.props.query, defaultQuery);
    const { server, capacity, timeoutS } = query;

    return (
      <div className="gf-form-group">
        <div className="gf-form">
          <FormField
            labelWidth={10}
            inputWidth={10}
            value={server}
            onChange={this.onServerChange}
            label="Server"
            tooltip="If not used the default server is used. Format: ws://SERVER:PORT"
          />
        </div>
        <div className="gf-form">
          <FormField
            label="Capacity"
            tooltip="Max. values"
            labelWidth={10}
            inputWidth={10}
            onChange={this.onCapacityChange}
            value={capacity}
            type="number"
          />
        </div>
        <div className="gf-form">
          <FormField
            label="Server Timeout (seconds)"
            labelWidth={13}
            inputWidth={7}
            tooltip="Enter 0 for no ping check"
            value={timeoutS}
            onChange={this.onTimeoutChange}
            type="number"
          />
        </div>
      </div>
    );
  }
}
