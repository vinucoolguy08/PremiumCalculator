import React, { Component } from 'react';
import axios from 'axios';

export class PremiumCalculator extends Component {
    constructor(props) {
        super(props);

        this.state = {
            name: '',
            age: '',
            dateOfBirth: '',
            occupation: '',
            deathSumInsured: '',
            premium: ''
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;
        this.setState({
            [name]: value
        });
        this.handleSubmit.bind(this);
    }
    
    handleSubmit(event) {
        event.preventDefault();
        axios.post('/premiumcalculator', {
            name: this.state.name,
            age: parseInt(this.state.age),
            dateOfBirth: new Date(this.state.dateOfBirth),
            occupation: this.state.occupation,
            deathCoverAmount: parseInt(this.state.deathSumInsured)
        })
        .then(response => {
            this.setState({
                premium: response.data
            });
        })
        .catch(error => {
            console.error(error);
        });
    }

    render() {
        return (
            <div>
                <h1>Premium Calculator</h1>
                <form onSubmit={this.handleSubmit}>
                    <div>
                        <label>Name:</label>
                        <input type="text" name="name" value={this.state.name} onChange={this.handleChange} required />
                    </div>
                    <div>
                        <label>Age:</label>
                        <input type="number" name="age" value={this.state.age} onChange={this.handleChange} required />
                    </div>
                    <div>
                        <label>Date of Birth:</label>
                        <input type="date" name="dateOfBirth" value={this.state.dateOfBirth} onChange={this.handleChange} required />
                    </div>
                    <div>
                        <label>Occupation:</label>
                        <select name="occupation" value={this.state.occupation} onChange={this.handleChange} required>
                            <option value="">Select Occupation</option>
                            <option value="Cleaner">Cleaner</option>
                            <option value="Doctor">Doctor</option>
                            <option value="Author">Author</option>
                            <option value="Farmer">Farmer</option>
                            <option value="Mechanic">Mechanic</option>
                            <option value="Florist">Florist</option>
                        </select>
                    </div>
                    <div>
                        <label>Death-Sum Insured:</label>
                        <input type="number" name="deathSumInsured" value={this.state.deathSumInsured} onChange={this.handleChange} required />
                    </div>
                    <div>
                        <button type="submit">Calculate Premium</button>
                    </div>
                </form>
                {this.state.premium !== '' && <div><strong>Monthly Premium:</strong> {this.state.premium}</div>}
            </div>
        );
    }
}
