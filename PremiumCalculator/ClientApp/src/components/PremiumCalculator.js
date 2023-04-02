import React, {Component} from 'react';
import axios from 'axios';

export class PremiumCalculator extends Component {
    constructor(props) {
        super(props);

        this.state = {
            name: '',
            age: '',
            dateOfBirth: '',
            occupation: '',
            sumInsured: '',
            premium: '',
            nameError: '',
            ageError: '',
            dateOfBirthError: '',
            occupationError: '',
            sumInsuredError: ''
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.calculatePremium = this.calculatePremium.bind(this);
        this.areAllStatesTruthy = this.areAllStatesTruthy.bind(this);
    }

    areAllStatesTruthy = () => {
        const errorSuffix = "Error";
        return Object.entries(this.state)
            .filter(([key, value]) => !key.endsWith(errorSuffix))
            .every(([key, value]) => value);
        // return Object.values(this.state).every(value => value);
    }

    handleChange(event) {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;
        this.setState({
            [name]: value
        });
        if (name === 'occupation' && this.areAllStatesTruthy()) {
            this.calculatePremium();
        }
    }

    handleSubmit(event) {
        event.preventDefault();
        this.calculatePremium();
    }

    calculatePremium() {
        this.setState({
            nameError: '',
            ageError: '',
            dateOfBirthError: '',
            occupationError: '',
            sumInsuredError: ''
        });
        axios.post('/premiumcalculator', {
            name: this.state.name,
            age: parseInt(this.state.age),
            dateOfBirth: new Date(this.state.dateOfBirth),
            occupation: this.state.occupation,
            sumInsured: parseInt(this.state.sumInsured)
        })
            .then(response => {
                this.setState({
                    premium: response.data
                });
            })
            .catch(error => {
                if (error.response && error.response.status === 400) {
                    const validationErrors = error.response.data.errors;
                    for (let fieldName in validationErrors) {
                        if (this.state.hasOwnProperty(fieldName)) {
                            this.setState({
                                [`${fieldName}Error`]: validationErrors[fieldName][0]
                            });
                        }
                    }
                } else {
                    console.error(error);
                }
            });
    }

    render() {
        return (
            <div>
                <h2>Premium Calculator</h2>
                <form onSubmit={this.handleSubmit}>
                    <div>
                        <label>Name:</label>
                        <input type="text" name="name" value={this.state.name} onChange={this.handleChange} required/>
                        {this.state.nameError && <div className="error">{this.state.nameError}</div>}
                    </div>
                    <div>
                        <label>Age:</label>
                        <input type="number" name="age" value={this.state.age} onChange={this.handleChange} required/>
                        {this.state.ageError && <div className="error">{this.state.ageError}</div>}
                    </div>
                    <div>
                        <label>Date of Birth:</label>
                        <input type="date" name="dateOfBirth" value={this.state.dateOfBirth}
                               onChange={this.handleChange} required/>
                        {this.state.dateOfBirthError && <div className="error">{this.state.dateOfBirthError}</div>}
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
                        {this.state.occupationError && <div className="error">{this.state.occupationError}</div>}
                    </div>
                    <div>
                        <label>Death-Sum Insured:</label>
                        <input type="number" name="sumInsured" value={this.state.sumInsured}
                               onChange={this.handleChange} required/>
                        {this.state.sumInsuredError && <div className="error">{this.state.sumInsuredError}</div>}
                    </div>
                    <div>
                        <button  className="calculate-premium" type="submit">Calculate Premium</button>
                    </div>
                </form>
                {this.state.premium !== '' && <div className="premium-display"><strong>Monthly Premium:</strong> {this.state.premium}</div>}
            </div>
        );
    }
}
